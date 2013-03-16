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
    class TestWindowCoveringControl
    {
        [Test]
        public void Can_open()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x60, 0x00);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.Open());
            }
        }

        [Test]
        public void Can_close()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x60, 0x01);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.Close());
            }
        }

        [Test]
        public void Can_stop()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x60, 0x02);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.Stop());
            }
        }

        [Test]
        public void Can_program()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x60, 0x03);

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.Program());
            }
        }

        [Test]
        public void Can_move_to_position()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario.SetupSendStandardCommandReceiveAck(0x61, 0x80); // half-way-open

                var test = buildObjectForTest(scenario.Playback());
                Assert.IsTrue(test.MoveToPosition(0x80));
            }
        }

        private static WindowCoveringControl buildObjectForTest(ISerialPortController serialPortController)
        {
            return TestDeviceHelper.BuildDeviceForTest<WindowCoveringControl>(0x0E, 0x00, serialPortController);
        }
    }
}
