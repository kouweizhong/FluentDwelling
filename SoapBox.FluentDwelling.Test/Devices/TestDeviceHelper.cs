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
using Rhino.Mocks;

namespace SoapBox.FluentDwelling.Test.Devices
{
    static class TestDeviceHelper
    {
        public const byte PLM_ID_HI = 0x01;
        public const byte PLM_ID_MI = 0x02;
        public const byte PLM_ID_LO = 0x03;

        public const byte PEER_ID_HI = 0x12;
        public const byte PEER_ID_MI = 0x34;
        public const byte PEER_ID_LO = 0x56;
        public const string PEER_ID_STRING = "12.34.56";

        public static T BuildDeviceForTest<T>(byte deviceCategory, byte deviceSubcategory, ISerialPortController serialPortController)
            where T : DeviceBase
        {
            return BuildDeviceForTest<T>(deviceCategory, deviceSubcategory, serialPortController, PEER_ID_HI, PEER_ID_MI, PEER_ID_LO);
        }

        public static T BuildDeviceForTest<T>(byte deviceCategory, byte deviceSubcategory, ISerialPortController serialPortController, 
            byte peerIdHi, byte peerIdMiddle, byte peerIdLo) where T : DeviceBase
        {
            const byte PEER_FIRMWARE = 0x00;
            const byte MSG_FLAGS = Constants.MSG_FLAGS_BROADCAST;

            var plm = new Plm(serialPortController);
            byte[] idRequestResponse = new byte[] { 0x02, 0x50, peerIdHi, peerIdMiddle, peerIdLo,
                deviceCategory, deviceSubcategory, PEER_FIRMWARE, MSG_FLAGS, 0x01, 0xFF};

            return DeviceFactory.BuildDevice(plm, idRequestResponse) as T;
        }

        public static void SetupSendStandardCommandReceiveAck(this SerialPortScenario scenario,
            byte command1, byte command2)
        {
            scenario
                .SetupSendStandardCommandReceiveAck(command1, command2, command1, command2);
        }

        public static void SetupSendStandardCommandReceiveAck(this SerialPortScenario scenario, 
            byte command1, byte command2, byte responseCommand1, byte responseCommand2)
        {
            const byte SEND_MESSAGE_FLAGS = Constants.MSG_FLAGS_DIRECT | Constants.MSG_FLAGS_MAX_HOPS;
            const byte RECV_MESSAGE_FLAGS_ACK = Constants.MSG_FLAGS_DIRECT_ACK;

            scenario
                .ShouldSend(0x02, 0x62, PEER_ID_HI, PEER_ID_MI, PEER_ID_LO, SEND_MESSAGE_FLAGS, command1, command2)
                .AndReceive(0x02, 0x62, PEER_ID_HI, PEER_ID_MI, PEER_ID_LO, SEND_MESSAGE_FLAGS, command1, command2, 0x06) // ack from PLM
                .WaitsForMessageOfType(0x50)
                .AndReceives(0x02, 0x50,
                    PEER_ID_HI, PEER_ID_MI, PEER_ID_LO,
                    PLM_ID_HI, PLM_ID_MI, PLM_ID_LO, RECV_MESSAGE_FLAGS_ACK, responseCommand1, responseCommand2); // ack from peer device

        }
    }
}
