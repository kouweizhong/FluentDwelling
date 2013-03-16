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
using Rhino.Mocks;

namespace SoapBox.FluentDwelling.Test
{
    [TestFixture]
    public class TestPlmLed
    {
        [Test]
        public void Can_enable_manual_control()
        {
            const byte INITIAL_CONFIGURATION = Constants.CONFIG_DISABLE_AUTO_LINKING;
            const byte NEW_CONFIGURATION = Constants.CONFIG_DISABLE_AUTO_LINKING | Constants.CONFIG_MANUAL_LED_CONTROL;

            testConfigurationChange(INITIAL_CONFIGURATION, NEW_CONFIGURATION, (PlmLed led) => led.EnableManualControl());
        }

        [Test]
        public void Can_disable_manual_control()
        {
            const byte INITIAL_CONFIGURATION = Constants.CONFIG_DISABLE_AUTO_LINKING | Constants.CONFIG_MANUAL_LED_CONTROL;
            const byte NEW_CONFIGURATION = Constants.CONFIG_DISABLE_AUTO_LINKING;

            testConfigurationChange(INITIAL_CONFIGURATION, NEW_CONFIGURATION, (PlmLed led) => led.DisableManualControl());
        }

        private void testConfigurationChange(byte initialConfiguration, byte newConfiguration, Func<PlmLed,PlmLed> testAction)
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x73)
                    .AndReceive(0x02, 0x73, initialConfiguration, 0x00, 0x00, Constants.ACK);
                scenario
                    .ShouldSend(0x02, 0x6B, newConfiguration)
                    .AndReceive(0x02, 0x6B, newConfiguration, Constants.ACK);

                var test = buildObjectForTest(scenario.Playback());
                Assert.AreSame(test, testAction(test));
            }
        }

        [Test]
        public void Can_turn_on_LED()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x6D)
                    .AndReceive(0x02, 0x6D, Constants.ACK);

                var test = buildObjectForTest(scenario.Playback());
                Assert.AreSame(test, test.TurnOn());
            }
        }

        [Test]
        public void Can_turn_off_LED()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x6E)
                    .AndReceive(0x02, 0x6E, Constants.ACK);

                var test = buildObjectForTest(scenario.Playback());
                Assert.AreSame(test, test.TurnOff());
            }
        }

        private PlmLed buildObjectForTest(ISerialPortController serialPortController)
        {
            return new Plm(serialPortController).Led;
        }
    }
}
