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
    class TestSensorsActuators
    {
        [Test]
        public void Can_turn_on_io_output()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x45, 0x02); // output 2 on

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOnOutput(2));
            }
        }

        [Test]
        public void Can_turn_off_io_output()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x46, 0x00); // output 0 off

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOffOutput(0));
            }
        }

        [Test]
        public void Can_write_output_port()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x48, 0xAF);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.WriteByteToOutputPort(0xAF));
            }
        }

        [Test]
        public void Can_get_input_port_values()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x49, 0x00, 0x49, 0xAB); // response with input status in command 2

                var test = buildObjectForTest(scenario.Playback());
                byte inputPortValue;
                Assert.IsTrue(test.TryGetInputPort(out inputPortValue));
                Assert.AreEqual(0xAB, inputPortValue);
            }
        }

        [Test]
        public void Can_get_sensor_value()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x4A, 0x03, 0x4A, 0xAB); // get sensor value 3

                var test = buildObjectForTest(scenario.Playback());
                byte sensorValue;
                Assert.IsTrue(test.TryGetSensorValue(3, out sensorValue));
                Assert.AreEqual(0xAB, sensorValue);
            }
        }

        private static SensorsActuators buildObjectForTest(ISerialPortController serialPortController)
        {
            return TestDeviceHelper.BuildDeviceForTest<SensorsActuators>(0x07, 0x00, serialPortController);
        }
    }
}
