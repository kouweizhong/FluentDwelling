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
    class TestDimmableLightingControl
    {
        [Test]
        public void Can_turn_on_to_level()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x12, 0x80);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOn(0x80));
            }
        }

        [Test]
        public void Can_ramp_on_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x11, 0x70);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.RampOn(0x70));
            }
        }

        [Test]
        public void Can_ramp_off_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x13, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.RampOff());
            }
        }

        [Test]
        public void Can_brighten_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x15, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.BrightenOneStep());
            }
        }

        [Test]
        public void Can_dim_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x16, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.DimOneStep());
            }
        }

        [Test]
        public void Can_manually_brighten_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x17, 0x01);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.BeginBrightening());
            }
        }

        [Test]
        public void Can_manually_dim_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x17, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.BeginDimming());
            }
        }

        [Test]
        public void Can_stop_manually_brightening_or_dimming_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x18, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.StopBrighteningOrDimming());
            }
        }

        private static DimmableLightingControl buildObjectForTest(ISerialPortController serialPortController)
        {
            return TestDeviceHelper.BuildDeviceForTest<DimmableLightingControl>(0x01, 0x00, serialPortController);
        }
    }
}
