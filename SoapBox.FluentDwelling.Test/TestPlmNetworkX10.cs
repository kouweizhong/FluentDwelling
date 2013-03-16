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

namespace SoapBox.FluentDwelling.Test
{
    [TestFixture]
    class TestPlmNetworkX10
    {
        [Test]
        public void Can_select_unit()
        {
            testHouseCodeAndUnit("A", 1, 0x66);
            testHouseCodeAndUnit("B", 2, 0xEE);
            testHouseCodeAndUnit("C", 3, 0x22);
            testHouseCodeAndUnit("D", 4, 0xAA);
            testHouseCodeAndUnit("E", 5, 0x11);
            testHouseCodeAndUnit("F", 6, 0x99);
            testHouseCodeAndUnit("G", 7, 0x55);
            testHouseCodeAndUnit("H", 8, 0xDD);
            testHouseCodeAndUnit("I", 9, 0x77);
            testHouseCodeAndUnit("J", 10, 0xFF);
            testHouseCodeAndUnit("K", 11, 0x33);
            testHouseCodeAndUnit("L", 12, 0xBB);
            testHouseCodeAndUnit("M", 13, 0x00);
            testHouseCodeAndUnit("N", 14, 0x88);
            testHouseCodeAndUnit("O", 15, 0x44);
            testHouseCodeAndUnit("P", 16, 0xCC);

            testHouseCodeAndUnit("A", 2, 0x6E);
            testHouseCodeAndUnit("M", 16, 0x0C);

            // should work with lower case
            testHouseCodeAndUnit("a", 1, 0x66);
        }

        private void testHouseCodeAndUnit(string houseCode, byte unitCode, byte x10Code)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x63, x10Code, 0x00)
                    .AndReceive(0x02, 0x63, x10Code, 0x00, Constants.ACK);

                var plm = buildPlmForTest(scenario.Playback());

                plm.Network.X10
                    .House(houseCode)
                    .Unit(unitCode);
            }
        }

        [Test]
        public void Can_send_command()
        {
            testX10Command("A", X10Command.AllLightsOff, 0x66);
            testX10Command("B", X10Command.StatusIsOff, 0xEE);
            testX10Command("C", X10Command.On, 0x22);
            testX10Command("D", X10Command.PresetDim1, 0xAA);
            testX10Command("E", X10Command.AllLightsOn, 0x11);
            testX10Command("F", X10Command.HailAcknowledge, 0x99);
            testX10Command("G", X10Command.Bright, 0x55);
            testX10Command("H", X10Command.StatusIsOn, 0xDD);
            testX10Command("I", X10Command.ExtendedCode, 0x77);
            testX10Command("J", X10Command.StatusRequest, 0xFF);
            testX10Command("K", X10Command.Off, 0x33);
            testX10Command("L", X10Command.PresetDim2, 0xBB);
            testX10Command("M", X10Command.AllUnitsOff, 0x00);
            testX10Command("N", X10Command.HailRequest, 0x88);
            testX10Command("O", X10Command.Dim, 0x44);
            testX10Command("P", X10Command.ExtendedData, 0xCC);

            testX10Command("g", X10Command.Dim, 0x54);
        }

        private void testX10Command(string houseCode, X10Command command, byte x10Code)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x63, x10Code, 0x80)
                    .AndReceive(0x02, 0x63, x10Code, 0x80, Constants.ACK);

                var plm = buildPlmForTest(scenario.Playback());

                plm.Network.X10
                    .House(houseCode)
                    .Command(command);
            }
        }

        [Test]
        public void Can_receive_X10_unit_codes()
        {
            testReceiveX10HouseAndUnit("A", 1, 0x66);
            testReceiveX10HouseAndUnit("B", 2, 0xEE);
            testReceiveX10HouseAndUnit("C", 3, 0x22);
            testReceiveX10HouseAndUnit("D", 4, 0xAA);
            testReceiveX10HouseAndUnit("E", 5, 0x11);
            testReceiveX10HouseAndUnit("F", 6, 0x99);
            testReceiveX10HouseAndUnit("G", 7, 0x55);
            testReceiveX10HouseAndUnit("H", 8, 0xDD);
            testReceiveX10HouseAndUnit("I", 9, 0x77);
            testReceiveX10HouseAndUnit("J", 10, 0xFF);
            testReceiveX10HouseAndUnit("K", 11, 0x33);
            testReceiveX10HouseAndUnit("L", 12, 0xBB);
            testReceiveX10HouseAndUnit("M", 13, 0x00);
            testReceiveX10HouseAndUnit("N", 14, 0x88);
            testReceiveX10HouseAndUnit("O", 15, 0x44);
            testReceiveX10HouseAndUnit("P", 16, 0xCC);

            testReceiveX10HouseAndUnit("A", 2, 0x6E);
            testReceiveX10HouseAndUnit("M", 16, 0x0C);
        }

        private void testReceiveX10HouseAndUnit(string houseCode, byte unitCode, byte x10Code)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x52, x10Code, 0x00);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.Network.X10;

                int eventCount = 0;
                object sender = null;
                string eventHouseCode = null;
                byte eventUnitCode = 0;
                test.UnitAddressed += new X10UnitAddressedHandler((s, e) =>
                {
                    sender = s;
                    eventHouseCode = e.HouseCode;
                    eventUnitCode = e.UnitCode;
                    eventCount++;
                });

                plm.Receive();
                Assert.AreEqual(1, eventCount);
                Assert.AreEqual(test, sender);
                Assert.AreEqual(houseCode, eventHouseCode);
                Assert.AreEqual(unitCode, eventUnitCode);
            }
        }

        [Test]
        public void Can_receive_X10_commands()
        {
            testReceiveX10Command("A", X10Command.AllLightsOff, 0x66);
            testReceiveX10Command("B", X10Command.StatusIsOff, 0xEE);
            testReceiveX10Command("C", X10Command.On, 0x22);
            testReceiveX10Command("D", X10Command.PresetDim1, 0xAA);
            testReceiveX10Command("E", X10Command.AllLightsOn, 0x11);
            testReceiveX10Command("F", X10Command.HailAcknowledge, 0x99);
            testReceiveX10Command("G", X10Command.Bright, 0x55);
            testReceiveX10Command("H", X10Command.StatusIsOn, 0xDD);
            testReceiveX10Command("I", X10Command.ExtendedCode, 0x77);
            testReceiveX10Command("J", X10Command.StatusRequest, 0xFF);
            testReceiveX10Command("K", X10Command.Off, 0x33);
            testReceiveX10Command("L", X10Command.PresetDim2, 0xBB);
            testReceiveX10Command("M", X10Command.AllUnitsOff, 0x00);
            testReceiveX10Command("N", X10Command.HailRequest, 0x88);
            testReceiveX10Command("O", X10Command.Dim, 0x44);
            testReceiveX10Command("P", X10Command.ExtendedData, 0xCC);

            testReceiveX10Command("G", X10Command.Dim, 0x54);
        }

        private void testReceiveX10Command(string houseCode, X10Command command, byte x10Code)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x52, x10Code, 0x80);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.Network.X10;

                int eventCount = 0;
                object sender = null;
                string eventHouseCode = null;
                X10Command eventCommand = 0;
                test.CommandReceived += new X10CommandReceivedHandler((s, e) =>
                {
                    sender = s;
                    eventHouseCode = e.HouseCode;
                    eventCommand = e.Command;
                    eventCount++;
                });

                plm.Receive();
                Assert.AreEqual(1, eventCount);
                Assert.AreEqual(test, sender);
                Assert.AreEqual(houseCode, eventHouseCode);
                Assert.AreEqual(command, eventCommand);
            }
        }

        private Plm buildPlmForTest(ISerialPortController serialPortController)
        {
            return new Plm(serialPortController);
        }
    }
}
