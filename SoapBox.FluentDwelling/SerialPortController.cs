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
using System.IO.Ports;
using System.Threading;

namespace SoapBox.FluentDwelling
{
    class SerialPortController : ISerialPortController
    {
        const int RECEIVE_TIMEOUT_MS = 1000;

        private readonly string comPortName;
        private SerialPort port;
        private readonly IncomingMessageBuffer buffer = new IncomingMessageBuffer();

        public SerialPortController(string comPortName)
        {
            if (comPortName == null) throw new ArgumentNullException("comPortName");
            this.comPortName = comPortName;
        }

        public byte[] SendReceive(byte[] send, int receiveLength)
        {
            byte[] result = new byte[0];
            serialPortTransaction(() =>
            {
                this.port.Write(send, 0, send.Length);
                byte messageType = send[1];
                result = waitForMessageOfType(messageType);
            });
            return result;
        }

        private byte[] waitForMessageOfType(params byte[] messageTypes)
        {
            receiveToBuffer();
            int msWaited = 0;
            IncomingMessage incomingMessage;
            while (!this.buffer.TryGetMessageByType(out incomingMessage, messageTypes))
            {
                if (msWaited >= RECEIVE_TIMEOUT_MS) throw new TimeoutException("Timeout waiting for receipt of data.");
                Thread.Sleep(10); msWaited += 10;
                receiveToBuffer();
            }
            return incomingMessage.Message;
        }

        public bool TryGetIncomingMessages(out byte[] message)
        {
            bool result = false;
            byte[] outMessage = null;
            serialPortTransaction(() =>
            {
                receiveToBuffer();
                IncomingMessage incomingMessage;
                if (this.buffer.TryGetIncomingMessage(out incomingMessage))
                {
                    outMessage = incomingMessage.Message;
                    result = true;
                }
            });
            message = outMessage;
            return result;
        }

        public byte[] GetIncomingMessageOfType(params byte[] messageTypes)
        {
            byte[] result = new byte[0];
            serialPortTransaction(() =>
            {
                result = waitForMessageOfType(messageTypes);
            });
            return result;
        }

        /// <summary>
        /// Copies bytes from the serial port into the buffer.
        /// Should be called inside a serial port transaction.
        /// </summary>
        private void receiveToBuffer()
        {
            byte[] incoming = new byte[256];
            if (this.port.BytesToRead > 0)
            {
                int bytesRead = this.port.Read(incoming, 0, 256);
                var read = incoming.Take(bytesRead).ToArray();
                this.buffer.AddBytes(read);
            }
        }

        /// <summary>
        /// Wraps the given action inside of some exception handling, and 
        /// some checking to make sure the port is open before use.
        /// </summary>
        /// <param name="action">Action working with the serial port</param>
        private void serialPortTransaction(Action action)
        {
            try
            {
                makeSurePortIsOpen();
                action();
            }
            catch (Exception)
            {
                this.buffer.Clear();
                closePortIfOpen();
                throw;
            }
        }

        private void openPort()
        {
            closePortIfOpen();
            this.port = new SerialPort(this.comPortName, 19200, Parity.None, 8, StopBits.One);
            this.port.Open();
            this.port.DiscardOutBuffer();
            this.port.DiscardInBuffer();
        }

        private void makeSurePortIsOpen()
        {
            if (this.port == null || !this.port.IsOpen)
            {
                openPort();
            }
        }

        private void closePortIfOpen()
        {
            if (this.port != null && this.port.IsOpen)
            {
                try
                {
                    this.port.Close();
                }
                catch (Exception) { }
            }
            this.port = null;
        }

        public void Dispose()
        {
            closePortIfOpen();
        }

    }
}
