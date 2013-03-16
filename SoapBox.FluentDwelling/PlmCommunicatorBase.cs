#region "SoapBox.FluentDwelling License"
/// <header module="SoapBox.FluentDwelling"> 
/// Copyright (C) 2011 SoapBox Automation Inc., All Rights Reserved.
/// Contact: SoapBox Automation Licencing (license@soapboxautomation.com)
/// 
/// This file is part of FluentDwelling.
/// 
/// FluentDwelling is free software: you can redistribute it and/or modify it
/// under the terms of the GNU General Public License as published by the 
/// Free Software Foundation, either version 3 of the License, or 
/// (at your option) any later version.
/// 
/// FluentDwelling is distributed in the hope that it will be useful, but 
/// WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along
/// with FluentDwelling. If not, see <http://www.gnu.org/licenses/>.
/// </header>
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoapBox.FluentDwelling
{
    public abstract class PlmCommunicatorBase : IDisposable
    {
        const byte MSG_TYPE_RECV_STANDARD = 0x50;
        const byte MSG_TYPE_RECV_EXTENDED = 0x51;

        private readonly ISerialPortController serialPortController;
        private readonly Queue<byte[]> receivedMessages = new Queue<byte[]>();

        internal PlmCommunicatorBase(ISerialPortController serialPortController)
        {
            if (serialPortController == null) throw new ArgumentNullException("serialPortController");
            this.serialPortController = serialPortController;
        }

        public bool Error { get; private set; }
        public Exception Exception { get; private set; }
        public event EventHandler OnError;

        private void raiseOnErrorEvent()
        {
            var evt = this.OnError;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        internal byte getIMConfiguration()
        {
            var result = this.serialPortController.SendReceive(new byte[] { 0x02, 0x73 }, 6);
            assertReceived(result, 0, 0x02);
            assertReceived(result, 1, 0x73);
            assertACK(result, 5);
            return result[2];
        }

        internal void setIMConfiguration(byte configuration)
        {
            sendCommandWithEchoAndAck(new byte[] { 0x02, 0x6B, configuration });
        }

        internal PlmInfo getIMInfo(Plm plm)
        {
            var result = this.serialPortController.SendReceive(new byte[] { 0x02, 0x60 }, 9);
            assertReceived(result, 0, 0x02);
            assertReceived(result, 1, 0x60);
            assertACK(result, 8);
            return new PlmInfo(plm, result);
        }

        internal PlmAllLinkDatabase getAllLinkDatabase()
        {
            var ackNack = sendCommandWithEchoReturnAck(new byte[] { 0x02, 0x69 }); // Get First Record
            var records = new List<PlmAllLinkDatabaseRecord>();
            while (ackNack == Constants.ACK)
            {
                byte[] record = this.serialPortController.GetIncomingMessageOfType(0x57);
                ackNack = sendCommandWithEchoReturnAck(new byte[] { 0x02, 0x6A }); // Get Next Record
                records.Add(new PlmAllLinkDatabaseRecord(record));
            }
            return new PlmAllLinkDatabase(records);
        }

        internal byte[] sendStandardLengthMessageAndWait4Response(DeviceId toAddress, byte flags, 
            byte command1, byte command2)
        {
            sendStandardLengthMessage(toAddress, (byte)(flags | Constants.MSG_FLAGS_MAX_HOPS), command1, command2);
            return waitForSpecificMessageFrom(toAddress, MSG_TYPE_RECV_STANDARD);
        }

        internal byte[] sendExtendedMessageAndWait4Response(DeviceId toAddress, byte flags,
              byte command1, byte command2)
        {
          sendExtendedMessage(toAddress, (byte)(flags | Constants.MSG_FLAGS_MAX_HOPS | Constants.MSG_FLAGS_EXTENDED), command1, command2);
          return waitForSpecificMessageFrom(toAddress, MSG_TYPE_RECV_EXTENDED);
        }

        internal byte[] waitForStandardMessageFrom(DeviceId peerAddress)
        {
            return waitForSpecificMessageFrom(peerAddress, MSG_TYPE_RECV_STANDARD);
        }

        internal byte[] waitForSpecificMessageFrom(DeviceId peerAddress, byte messageType)
        {
          byte[] result = null;
          int tryCounter = 0;
          DeviceId originator;
          do
          {
            result = this.serialPortController.GetIncomingMessageOfType(messageType);
            originator = DeviceMessage.DeviceMessageOriginator(result);
            if (originator != peerAddress)
            {
              queueReceivedMessage(result);
              result = null;
              tryCounter++;
              if (tryCounter > 3)
              {
                throw new TimeoutException("Timed out waiting for response from " + peerAddress.ToString());
              }
            }
          } while (result == null);
          return result;
        }

        internal void receiveAndQueueIncomingMessage()
        {
            byte[] message;
            if (this.serialPortController.TryGetIncomingMessages(out message))
            {
                queueReceivedMessage(message);
            }
        }

        private void queueReceivedMessage(byte[] message)
        {
            this.receivedMessages.Enqueue(message);
        }

        internal void processReceivedMessageQueue()
        {
            while (this.receivedMessages.Count > 0)
            {
                var message = this.receivedMessages.Dequeue();
                processIncomingMessage(message);
            }
        }

        protected virtual void processIncomingMessage(byte[] message) { }

        internal void sendStandardLengthMessage(DeviceId toAddress, byte flags, 
            byte command1, byte command2)
        {
            sendCommandWithEchoAndAck(0x02, 0x62,
                toAddress.IdHi, toAddress.IdMiddle, toAddress.IdLo,
                flags, command1, command2);
        }

        internal void sendExtendedMessage(DeviceId toAddress, byte flags,
              byte command1, byte command2)
        {
          sendCommandWithEchoAndAck(0x02, 0x62,
                                    toAddress.IdHi, toAddress.IdMiddle, toAddress.IdLo,
                                    flags, command1, command2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// Sends a sequence of bytes to the PLM, and verifies
        /// that the PLM echos back the command, plus an ACK.
        /// Throws an exception if it doesn't.
        /// </summary>
        /// <param name="send">Bytes to send</param>
        internal void sendCommandWithEchoAndAck(params byte[] send)
        {
            var ackNack = sendCommandWithEchoReturnAck(send);
            assertACK(ackNack);
        }

        /// <summary>
        /// Sends a sequence of bytes to thePLM, and verifies
        /// that the PLM echos back the command.  Assumes there
        /// is one more byte (an ACK/NACK) but doesn't test it,
        /// just returns it for inspection.
        /// </summary>
        /// <param name="send"></param>
        /// <returns></returns>
        private byte sendCommandWithEchoReturnAck(params byte[] send)
        {
            var result = this.serialPortController.SendReceive(send, send.Length + 1);
            for (int i = 0; i < send.Length; i++)
            {
                assertReceived(result, i, send[i]);
            }
            return result[send.Length];
        }

        protected void assertReceived(byte[] received, int byteIndex, byte shouldBe)
        {
            byte receivedByte = received[byteIndex];
            if (receivedByte != shouldBe)
            {
                throw new PlmException("Received data byte index " + byteIndex.ToString() +
                    " was 0x" + receivedByte.ToString("X2") +
                    " but should be 0x" + shouldBe.ToString("X2") + ".");
            }
        }

        protected void assertACK(byte[] received, int byteIndex)
        {
            byte receivedByte = received[byteIndex];
            if (receivedByte != Constants.ACK)
            {
                throw new PlmException("Received data byte index " + byteIndex.ToString() +
                    " was 0x" + receivedByte.ToString("X2") +
                    " but should be an ACK (0x" + Constants.ACK.ToString("X2") + ").");
            }
        }

        protected void assertACK(int ackNack)
        {
            if (ackNack != Constants.ACK)
            {
                throw new PlmException("Received data byte " + 
                    " was 0x" + ackNack.ToString("X2") +
                    " but should be an ACK (0x" + Constants.ACK.ToString("X2") + ").");
            }
        }

        /// <summary>
        /// Wraps the given action in an exception handler, and
        /// captures any exception, putting it in the status properties.
        /// </summary>
        /// <param name="action">Action that could generate an exception</param>
        internal void exceptionHandler(Action action)
        {
            bool raise = false;
            try
            {
                this.Error = false;
                action();
            }
            catch (Exception ex)
            {
                this.Error = true;
                this.Exception = ex;
                raise = true;
            }
            if (raise) raiseOnErrorEvent();
        }

        public void Dispose()
        {
            this.serialPortController.Dispose();
        }
    }
}
