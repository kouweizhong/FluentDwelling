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
    class TestLightingControl
    {
        [Test]
        public void Can_turn_on_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x12, 0xFF);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOn());
            }
        }

        [Test]
        public void Can_turn_off_light()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x14, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.TurnOff());
            }
        }

        [Test]
        public void Can_get_on_level()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x19, 0x00, 0x11, 0x55);

                var test = buildObjectForTest(scenario.Playback());
                byte onLevel;
                Assert.IsTrue(test.TryGetOnLevel(out onLevel));
                Assert.AreEqual(0x55, onLevel);
            }
        }

        private static LightingControl buildObjectForTest(ISerialPortController serialPortController)
        {
            return TestDeviceHelper.BuildDeviceForTest<LightingControl>(0x01, 0x00, serialPortController);
        }

    }
}
