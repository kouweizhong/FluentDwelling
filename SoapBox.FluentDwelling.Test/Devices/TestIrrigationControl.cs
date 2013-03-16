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
    class TestIrrigationControl
    {
        [Test]
        public void Can_turn_on_sprinkler_valve()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x40, 0x03); // valve 3 on

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOnSprinklerValve(3));
            }
        }

        [Test]
        public void Can_turn_off_sprinkler_valve()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x41, 0x05); // valve 5 off

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOffSprinklerValve(5));
            }
        }

        [Test]
        public void Can_turn_on_sprinkler_program()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x42, 0x07); // program 7 on

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOnSprinklerProgram(7));
            }
        }

        [Test]
        public void Can_turn_off_sprinkler_program()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x43, 0x09); // program 9 off

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOffSprinklerProgram(9));
            }
        }

        [Test]
        public void Can_load_initialization_values()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.LoadInitializationValues());
            }
        }

        [Test]
        public void Can_load_eeprom_from_ram()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x01);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.LoadEepromFromRam());
            }
        }

        [Test]
        public void Can_get_valve_status()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x02, 0x44, 0xAB); // response with valve status in command 2

                var test = buildObjectForTest(scenario.Playback());
                byte valveStatus;
                Assert.IsTrue(test.TryGetValveStatus(out valveStatus));
                Assert.AreEqual(0xAB, valveStatus);
            }
        }

        [Test]
        public void Can_inhibit_command_acceptance()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x03);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.InhibitCommandAcceptance());
            }
        }

        [Test]
        public void Can_resume_command_acceptance()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x04);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.ResumeCommandAcceptance());
            }
        }

        [Test]
        public void Can_skip_forward()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x05);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.SkipForward());
            }
        }

        [Test]
        public void Can_skip_back()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x06);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.SkipBack());
            }
        }

        [Test]
        public void Can_enable_pump_on_V8()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x07);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.EnablePumpOnV8());
            }
        }

        [Test]
        public void Can_disable_pump_on_V8()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x08);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.DisablePumpOnV8());
            }
        }

        [Test]
        public void Can_enable_device_status_changed_broadcast()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x09);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.EnableDeviceStatusChangedBroadcast());
            }
        }

        [Test]
        public void Can_disable_device_status_changed_broadcast()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x0A);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.DisableDeviceStatusChangedBroadcast());
            }
        }

        [Test]
        public void Can_load_ram_from_eeprom()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x0B);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.LoadRamFromEeprom());
            }
        }

        [Test]
        public void Can_enable_sensor_reading()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x0C);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.EnableSensorReading());
            }
        }

        [Test]
        public void Can_disable_sensor_reading()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x0D);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.DisableSensorReading());
            }
        }

        [Test]
        public void Can_put_device_in_self_diagnostics_mode()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x0E);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.DiagnosticsOn());
            }
        }

        [Test]
        public void Can_take_device_out_of_self_diagnostics_mode()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x44, 0x0F);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.DiagnosticsOff());
            }
        }

        private static IrrigationControl buildObjectForTest(ISerialPortController serialPortController)
        {
            return TestDeviceHelper.BuildDeviceForTest<IrrigationControl>(0x04, 0x00, serialPortController);
        }
    }
}
