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
using SoapBox.FluentDwelling.Devices;

namespace SoapBox.FluentDwelling
{
    /// <summary>
    /// Represents the network interface on the
    /// PLM (both RF and power-line).
    /// </summary>
    public class PlmNetwork
    {
        private readonly Plm plm;
        private readonly PlmNetworkX10 x10;
        private readonly Dictionary<DeviceId, DeviceBase> deviceCache = new Dictionary<DeviceId, DeviceBase>();

        internal PlmNetwork(Plm plm)
        {
            if (plm == null) throw new ArgumentNullException("plm");
            this.plm = plm;
            this.x10 = new PlmNetworkX10(this.plm);
        }

        public PlmNetworkX10 X10 { get { return this.x10; } }

        /// <summary>
        /// Sends the given standard command to the given address
        /// and returns true if the peer responded with an ACK.
        /// </summary>
        /// <param name="dottedHexId">Example: "12.34.56"</param>
        public bool SendStandardCommandToAddress(string dottedHexId, byte command1, byte command2)
        {
            byte[] rawResponse;
            return SendStandardCommandToAddress(dottedHexId, command1, command2, out rawResponse);
        }
        
        /// <summary>
        /// Sends the given standard command to the given address
        /// and returns true if the peer responded with an ACK.
        /// This overload returns the raw response data in case you need it.
        /// </summary>
        /// <param name="dottedHexId">Example: "12.34.56"</param>
        public bool SendStandardCommandToAddress(string dottedHexId, byte command1, byte command2, out byte[] rawResponse)
        {
            var deviceId = new DeviceId(dottedHexId);
            return SendStandardCommandToAddress(deviceId, command1, command2, out rawResponse);
        }

        /// <summary>
        /// Sends the given standard command to the given address
        /// and returns true if the peer responded with an ACK.
        /// </summary>
        /// <param name="toAddress">Example: new DeviceId("12.34.56")</param>
        public bool SendStandardCommandToAddress(DeviceId toAddress, byte command1, byte command2)
        {
            byte[] rawResponse;
            return SendStandardCommandToAddress(toAddress, command1, command2, out rawResponse);
        }

        /// <summary>
        /// Sends the given standard command to the given address
        /// and returns true if the peer responded with an ACK.
        /// This overload returns the raw response data in case you need it.
        /// </summary>
        /// <param name="toAddress">Example: new DeviceId("12.34.56")</param>
        public bool SendStandardCommandToAddress(DeviceId toAddress, byte command1, byte command2, out byte[] rawResponse)
        {
            bool result = false;
            byte[] responseAck = new byte[] {};
            this.plm.exceptionHandler(() =>
            {
                responseAck = this.plm.sendStandardLengthMessageAndWait4Response(
                        toAddress, Constants.MSG_FLAGS_DIRECT, command1, command2);
                byte flags = DeviceMessage.MessageFlags(responseAck);
                result = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
            });
            rawResponse = responseAck;
            return result;
        }

        /// <summary>
        /// Sends a message to the device with the given
        /// DeviceId in order to try to identify it, and 
        /// if successful, returns a DeviceBase object.
        /// The derived type of the returned object varies
        /// depending on the Device category discovered 
        /// during the identification process.
        /// </summary>
        /// <param name="dottedHexId">Example: "12.34.56"</param>
        /// <returns>null if unsuccessful - check plm.Exception</returns>
        public bool TryConnectToDevice(string dottedHexId, out DeviceBase device)
        {
            var peerId = new DeviceId(dottedHexId);
            return TryConnectToDevice(peerId, out device);
        }

        /// <summary>
        /// Sends a message to the device with the given
        /// DeviceId in order to try to identify it, and 
        /// if successful, returns a DeviceBase object.
        /// The derived type of the returned object varies
        /// depending on the Device category discovered 
        /// during the identification process.
        /// </summary>
        /// <returns>null if unsuccessful - check plm.Exception</returns>
        public bool TryConnectToDevice(DeviceId deviceId, out DeviceBase device)
        {
            if(deviceId == null) throw new ArgumentNullException("deviceId");
            DeviceBase result = null;
            if (this.deviceCache.ContainsKey(deviceId))
            {
                result = this.deviceCache[deviceId];
            }
            else
            {
                this.plm.exceptionHandler(() =>
                    {
                        byte[] responseAck = this.plm.sendStandardLengthMessageAndWait4Response(
                            deviceId, Constants.MSG_FLAGS_DIRECT, 0x10, 0x00);
                        byte[] responseIdRequest = this.plm.waitForStandardMessageFrom(deviceId);
                        result = DeviceFactory.BuildDevice(this.plm, responseIdRequest);
                    });
                if (result != null)
                {
                    this.deviceCache.Add(deviceId, result);
                }
            }
            device = result;
            return result != null;
        }

        /// <summary>
        /// This event is fired if someone "taps" the SET button
        /// on the PLM.  A tap is when they press the button
        /// but release it before entering all-linking mode.
        /// </summary>
        public event AllLinkingCompletedHandler AllLinkingCompleted;

        private void fireAllLinkingCompletedEvent(byte linkCode, byte group, DeviceId peerId,
            byte deviceCategory, byte deviceSubcategory)
        {
            var evt = AllLinkingCompleted;
            if (evt != null)
            {
                evt(this, new AllLinkingCompletedArgs(linkCode, group, peerId, 
                    deviceCategory, deviceSubcategory));
            }
        }

        internal void allLinkingCompleted(byte linkCode, byte group, DeviceId peerId, 
            byte deviceCategory, byte deviceSubcategory)
        {
            fireAllLinkingCompletedEvent(linkCode, group, peerId, deviceCategory, deviceSubcategory);
        }

        /// <summary>
        /// This event is fired if the PLM receives a standard message from
        /// another device (such as Turn On Light, etc.).
        /// </summary>
        public event StandardMessageReceivedHandler StandardMessageReceived;

        private void fireStandardMessageReceivedEvent(DeviceId peerId, byte group,
            byte flags, byte command1, byte command2)
        {
            var evt = StandardMessageReceived;
            if (evt != null)
            {
                evt(this, new StandardMessageReceivedArgs(peerId,
                    group, flags, command1, command2));
            }
        }

        internal void standardMessageReceived(DeviceId peerId, byte group,
            byte flags, byte command1, byte command2)
        {
            fireStandardMessageReceivedEvent(peerId, group, flags, command1, command2);
        }
    }
}
