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
using NUnit.Framework;
using SoapBox.FluentDwelling.Devices;

namespace SoapBox.FluentDwelling.Test.Devices
{
    [TestFixture]
    public class TestDeviceBase
    {
        const byte PLM_ID_HI = 0x01;
        const byte PLM_ID_MI = 0x02;
        const byte PLM_ID_LO = 0x03;

        const byte PEER_ID_HI = 0x12;
        const byte PEER_ID_MI = 0x34;
        const byte PEER_ID_LO = 0x56;

        [Test]
        public void Can_ping_device()
        {
            testPing(pingResponse: true);
            testPing(pingResponse: false);
        }

        private static void testPing(bool pingResponse)
        {
            using (var scenario = new SerialPortScenario())
            {
                const byte SEND_MESSAGE_FLAGS = Constants.MSG_FLAGS_DIRECT | Constants.MSG_FLAGS_MAX_HOPS;
                const byte RECV_MESSAGE_FLAGS_ACK = Constants.MSG_FLAGS_DIRECT_ACK;

                byte[] peerResponse = new byte[] { 0x02, 0x50,
                        PEER_ID_HI, PEER_ID_MI, PEER_ID_LO,
                        PLM_ID_HI, PLM_ID_MI, PLM_ID_LO, RECV_MESSAGE_FLAGS_ACK, 0x0F, 0x00 };

                scenario
                    .ShouldSend(0x02, 0x62, PEER_ID_HI, PEER_ID_MI, PEER_ID_LO, SEND_MESSAGE_FLAGS, 0x0F, 0x00)
                    .AndReceive(0x02, 0x62, PEER_ID_HI, PEER_ID_MI, PEER_ID_LO, SEND_MESSAGE_FLAGS, 0x0F, 0x00, 0x06);
                if (pingResponse)
                {
                    scenario
                        .WaitsForMessageOfType(0x50)
                        .AndReceives(peerResponse);
                }
                else
                {
                    scenario
                        .WaitsForMessageOfType(0x50)
                        .AndReceives();
                }

                var test = buildObjectForTest(scenario.Playback());
                Assert.AreEqual(pingResponse, test.Ping());
            }
        }

        private static DeviceBase buildObjectForTest(ISerialPortController serialPortController)
        {
            return TestDeviceHelper.BuildDeviceForTest<DeviceBase>(0x00, 0x00, serialPortController,
                PEER_ID_HI, PEER_ID_MI, PEER_ID_LO);
        }
    }
}
