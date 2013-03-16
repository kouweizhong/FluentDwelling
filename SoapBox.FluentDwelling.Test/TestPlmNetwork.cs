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
using SoapBox.FluentDwelling.Test.Devices;

namespace SoapBox.FluentDwelling.Test
{
    [TestFixture]
    public class TestPlmNetwork
    {
        const byte PEER_PK_MSB = 0x00;
        const byte PEER_PK_2MSB = 0x00;
        const byte PEER_PK_LSB = 0x1E;
        const byte PEER_DEVCAT = 0x01; // dimmable lighting
        const byte PEER_DEVSUB = 0x0D; // socketlinc
        const byte PEER_FIRMWARE = 0xAA;

        [Test]
        public void Can_connect_to_device_by_Id()
        {
            DeviceId peerId = new DeviceId(
                TestDeviceHelper.PEER_ID_HI,
                TestDeviceHelper.PEER_ID_MI,
                TestDeviceHelper.PEER_ID_LO);
            // can get using an existing DeviceId
            testGettingDevice(test =>
            {
                DeviceBase result;
                Assert.IsTrue(test.TryConnectToDevice(peerId, out result));
                Assert.IsNotNull(result);
                return result;
            });
            // convenience method: can get device using a string
            testGettingDevice(test =>
            {
                DeviceBase result;
                Assert.IsTrue(test.TryConnectToDevice(TestDeviceHelper.PEER_ID_STRING, out result));
                Assert.IsNotNull(result);
                return result;
            });
        }

        private void testGettingDevice(Func<PlmNetwork, DeviceBase> testFunc)
        {
            using (var scenario = new SerialPortScenario())
            {
                const byte RECV_MESSAGE_FLAGS_SB = Constants.MSG_FLAGS_BROADCAST;

                scenario.SetupSendStandardCommandReceiveAck(0x10, 0x00);

                scenario
                    .WaitsForMessageOfType(0x50)
                    .AndReceives(0x02, 0x50,
                        TestDeviceHelper.PEER_ID_HI, TestDeviceHelper.PEER_ID_MI, TestDeviceHelper.PEER_ID_LO,
                        PEER_DEVCAT, PEER_DEVSUB, PEER_FIRMWARE, RECV_MESSAGE_FLAGS_SB, 0x01, 0xFF);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.Network;

                var device = testFunc(test);
                Assert.IsNotNull(device);
                Assert.AreEqual(TestDeviceHelper.PEER_ID_STRING, device.DeviceId.ToString());
                string devCat = Constants.DeviceCategoryLookup[PEER_DEVCAT];
                string devSubcat = Constants.DeviceSubcategoryLookup[PEER_DEVCAT][PEER_DEVSUB];
                Assert.AreEqual(devCat, device.DeviceCategory);
                Assert.AreEqual(devSubcat, device.DeviceSubcategory);
                Assert.IsInstanceOf<LightingControl>(device);
                Assert.IsInstanceOf<DimmableLightingControl>(device);
            }
        }

        [Test]
        public void Caches_devices_and_returns_same_object_if_called_twice()
        {
            using (var scenario = new SerialPortScenario())
            {
                const byte RECV_MESSAGE_FLAGS_SB = Constants.MSG_FLAGS_BROADCAST;

                scenario.SetupSendStandardCommandReceiveAck(0x10, 0x00);

                scenario
                    .WaitsForMessageOfType(0x50)
                    .AndReceives(0x02, 0x50,
                        TestDeviceHelper.PEER_ID_HI, TestDeviceHelper.PEER_ID_MI, TestDeviceHelper.PEER_ID_LO,
                        PEER_DEVCAT, PEER_DEVSUB, PEER_FIRMWARE, RECV_MESSAGE_FLAGS_SB, 0x01, 0xFF);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.Network;

                DeviceBase firstDevice;
                DeviceBase secondDevice;
                Assert.IsTrue(test.TryConnectToDevice(TestDeviceHelper.PEER_ID_STRING, out firstDevice));
                Assert.IsTrue(test.TryConnectToDevice(TestDeviceHelper.PEER_ID_STRING, out secondDevice));
                Assert.AreSame(firstDevice, secondDevice);
            }
        }

        [Test]
        public void Fires_event_when_all_linking_completed()
        {
            testAllLinkingCompletedEvent(0x00, 0x01);
            testAllLinkingCompletedEvent(0x01, 0x02);
            testAllLinkingCompletedEvent(0xFF, 0x03);
        }

        public void testAllLinkingCompletedEvent(byte linkCode, byte group)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x53, linkCode, group,
                        TestDeviceHelper.PEER_ID_HI,
                        TestDeviceHelper.PEER_ID_MI,
                        TestDeviceHelper.PEER_ID_LO,
                        PEER_DEVCAT,
                        PEER_DEVSUB,
                        0xFF);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.Network;

                int eventCount = 0;
                test.AllLinkingCompleted += new AllLinkingCompletedHandler((s, e) =>
                {
                    Assert.AreSame(test, s);
                    Assert.AreEqual(group, e.AllLinkGroup);
                    switch (linkCode)
                    {
                        case 0x00:
                            Assert.AreEqual(AllLinkingAction.LinkedWithPlmAsSlave, e.AllLinkingAction);
                            break;
                        case 0x01:
                            Assert.AreEqual(AllLinkingAction.LinkedWithPlmAsMaster, e.AllLinkingAction);
                            break;
                        case 0xFF:
                            Assert.AreEqual(AllLinkingAction.LinkDeleted, e.AllLinkingAction);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    Assert.AreEqual(TestDeviceHelper.PEER_ID_STRING, e.PeerId.ToString());
                    Assert.AreEqual(PEER_DEVCAT, e.DeviceCategoryCode);
                    Assert.AreEqual(PEER_DEVSUB, e.DeviceSubcategoryCode);
                    string devCat = Constants.DeviceCategoryLookup[PEER_DEVCAT];
                    string devSubcat = Constants.DeviceSubcategoryLookup[PEER_DEVCAT][PEER_DEVSUB];
                    Assert.AreEqual(devCat, e.DeviceCategory);
                    Assert.AreEqual(devSubcat, e.DeviceSubcategory);
                    eventCount++;
                });

                plm.Receive();
                Assert.AreEqual(1, eventCount);
            }
        }

        [Test]
        public void Can_get_X10_context()
        {
            using (var scenario = new SerialPortScenario())
            {
                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.Network;
                PlmNetworkX10 x10 = test.X10;
            }
        }

        [Test]
        public void Fires_event_when_standard_message_received()
        {
            testStandardMessageReceivedEvent(0x0F, 0x00); // ping
        }

        private void testStandardMessageReceivedEvent(byte command1, byte command2)
        {
            const byte FLAGS = Constants.MSG_FLAGS_GROUP_BROADCAST | Constants.MSG_FLAGS_MAX_HOPS;
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x50,
                        TestDeviceHelper.PEER_ID_HI,
                        TestDeviceHelper.PEER_ID_MI,
                        TestDeviceHelper.PEER_ID_LO,
                        0x00, // ignored
                        0x00, // ignored
                        0x04, // ignored
                        FLAGS,
                        command1,
                        command2);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.Network;

                int eventCount = 0;
                test.StandardMessageReceived += new StandardMessageReceivedHandler((s, e) =>
                {
                    Assert.AreSame(test, s);
                    Assert.AreEqual(FLAGS, e.RawFlags);
                    Assert.AreEqual(command1, e.Command1);
                    Assert.AreEqual(command2, e.Command2);

                    Assert.AreEqual(TestDeviceHelper.PEER_ID_STRING, e.PeerId.ToString());
                    eventCount++;
                });

                plm.Receive();
                Assert.AreEqual(1, eventCount);
            }
        }

        private Plm buildPlmForTest(ISerialPortController serialPortController)
        {
            return new Plm(serialPortController);
        }
    }
}
