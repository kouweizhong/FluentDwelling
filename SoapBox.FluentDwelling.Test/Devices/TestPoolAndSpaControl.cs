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
    class TestPoolAndSpaControl
    {
        [Test]
        public void Can_turn_on_device()
        {
            testTurnOnPoolAndSpaDevice(0x01, PoolAndSpaDevice.Pool);
            testTurnOnPoolAndSpaDevice(0x02, PoolAndSpaDevice.Spa);
            testTurnOnPoolAndSpaDevice(0x03, PoolAndSpaDevice.Heat);
            testTurnOnPoolAndSpaDevice(0x04, PoolAndSpaDevice.Pump);

            // test invalid input
            using (var scenario = new SerialPortScenario())
            {
                var test = buildObjectForTest(scenario.Playback());
                Assert.Throws<ArgumentOutOfRangeException>(() => test.TurnOnDevice(PoolAndSpaDevice.All));
            }
        }

        private static void testTurnOnPoolAndSpaDevice(byte deviceCode, PoolAndSpaDevice device)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x50, deviceCode);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOnDevice(device));
            }
        }

        [Test]
        public void Can_turn_on_device_by_number()
        {
            for (int i = 1; i <= 255; i++)
            {
                testTurnOnPoolAndSpaDevice((byte)i);
            }

            // test invalid input
            using (var scenario = new SerialPortScenario())
            {
                var test = buildObjectForTest(scenario.Playback());
                Assert.Throws<ArgumentOutOfRangeException>(() => test.TurnOnDevice((byte)0));
            }
        }

        private static void testTurnOnPoolAndSpaDevice(byte deviceCode)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x50, deviceCode);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOnDevice(deviceCode));
            }
        }

        [Test]
        public void Can_turn_off_device()
        {
            testTurnOffPoolAndSpaDevice(0x00, PoolAndSpaDevice.All);
            testTurnOffPoolAndSpaDevice(0x01, PoolAndSpaDevice.Pool);
            testTurnOffPoolAndSpaDevice(0x02, PoolAndSpaDevice.Spa);
            testTurnOffPoolAndSpaDevice(0x03, PoolAndSpaDevice.Heat);
            testTurnOffPoolAndSpaDevice(0x04, PoolAndSpaDevice.Pump);
        }

        private static void testTurnOffPoolAndSpaDevice(byte deviceCode, PoolAndSpaDevice device)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x51, deviceCode);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOffDevice(device));
            }
        }

        [Test]
        public void Can_turn_off_device_by_number()
        {
            for (int i = 0; i <= 255; i++)
            {
                testTurnOffPoolAndSpaDevice((byte)i);
            }
        }

        private static void testTurnOffPoolAndSpaDevice(byte deviceCode)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x51, deviceCode);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOffDevice(deviceCode));
            }
        }

        [Test]
        public void Can_raise_temperature_setting()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x52, 0x03);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.RaiseTemperatureSettingBy(0x03));
            }
        }

        [Test]
        public void Can_lower_temperature_setting()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x53, 0x04);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.LowerTemperatureSettingBy(0x04));
            }
        }

        [Test]
        public void Can_load_initialization_values()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x54, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.LoadInitializationValues());
            }
        }

        [Test]
        public void Can_load_eeprom_from_ram()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x54, 0x01);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.LoadEepromFromRam());
            }
        }

        [Test]
        public void Can_get_thermostat_mode()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x54, 0x02, 0x54, 0x01); // response with thermostat in spa mode

                var test = buildObjectForTest(scenario.Playback());
                PoolAndSpaThermostatMode thermostatMode;
                Assert.IsTrue(test.TryGetThermostatMode(out thermostatMode));
                Assert.AreEqual(PoolAndSpaThermostatMode.Spa, thermostatMode);
            } 
            
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x54, 0x02, 0x54, 0x00); // response with thermostat in pool mode

                var test = buildObjectForTest(scenario.Playback());
                PoolAndSpaThermostatMode thermostatMode;
                Assert.IsTrue(test.TryGetThermostatMode(out thermostatMode));
                Assert.AreEqual(PoolAndSpaThermostatMode.Pool, thermostatMode);
            }
        }

        [Test]
        public void Can_get_ambient_temperature()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x54, 0x03, 0x54, 0x05);

                var test = buildObjectForTest(scenario.Playback());
                byte ambientTemperature;
                Assert.IsTrue(test.TryGetAmbientTemperature(out ambientTemperature));
                Assert.AreEqual(0x05, ambientTemperature);
            }
        }

        [Test]
        public void Can_get_water_temperature()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x54, 0x04, 0x54, 0x18);

                var test = buildObjectForTest(scenario.Playback());
                byte waterTemperature;
                Assert.IsTrue(test.TryGetWaterTemperature(out waterTemperature));
                Assert.AreEqual(0x18, waterTemperature);
            }
        }

        [Test]
        public void Can_get_pH()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x54, 0x05, 0x54, 0x55);

                var test = buildObjectForTest(scenario.Playback());
                byte pH;
                Assert.IsTrue(test.TryGetPH(out pH));
                Assert.AreEqual(0x55, pH);
            }
        }

        private static PoolAndSpaControl buildObjectForTest(ISerialPortController serialPortController)
        {
            return TestDeviceHelper.BuildDeviceForTest<PoolAndSpaControl>(0x06, 0x00, serialPortController);
        }
    }
}
