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
    class TestPlmSetButton
    {
        [Test]
        public void Fires_event_when_set_button_tapped()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x54, 0x02);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.SetButton;

                int eventCount = 0;
                test.Tapped += new EventHandler((s, e) =>
                {
                    Assert.AreSame(test, s);
                    eventCount++;
                });

                plm.Receive();
                Assert.AreEqual(1, eventCount);
            }
        }

        [Test]
        public void Fires_event_when_set_button_pressed_and_held()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x54, 0x03);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.SetButton;

                int eventCount = 0;
                test.PressedAndHeld += new EventHandler((s, e) =>
                {
                    Assert.AreSame(test, s);
                    eventCount++;
                });

                plm.Receive();
                Assert.AreEqual(1, eventCount);
            }
        }

        [Test]
        public void Fires_event_when_set_button_released_after_being_held()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x54, 0x04);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.SetButton;

                int eventCount = 0;
                test.ReleasedAfterHolding += new EventHandler((s, e) =>
                {
                    Assert.AreSame(test, s);
                    eventCount++;
                });

                plm.Receive();
                Assert.AreEqual(1, eventCount);
            }
        }

        [Test]
        public void Fires_event_when_set_button_held_during_power_up()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .IncomingMessage(0x02, 0x55);

                var plm = buildPlmForTest(scenario.Playback());
                var test = plm.SetButton;

                int eventCount = 0;
                test.UserReset += new EventHandler((s, e) =>
                {
                    Assert.AreSame(test, s);
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
