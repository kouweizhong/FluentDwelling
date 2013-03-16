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
    public class TestPlm
    {
        [Test]
        public void Can_get_configuration()
        {
            const byte CONFIGURATION = Constants.CONFIG_MANUAL_LED_CONTROL | Constants.CONFIG_DISABLE_AUTO_LINKING;

            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x73)
                    .AndReceive(0x02, 0x73, CONFIGURATION, 0x00, 0x00, Constants.ACK);

                var test = buildObjectForTest(scenario.Playback());
                var configuration = test.GetConfiguration();

                Assert.IsTrue(configuration.AutoLinkingDisabled);
                Assert.IsFalse(configuration.MonitorMode);
                Assert.IsTrue(configuration.ManualLedControl);
                Assert.IsFalse(configuration.Rs232Deadman);
            }
        }

        [Test]
        public void Can_get_info()
        {
            const byte ID_HI = 0x08;
            const byte ID_MIDDLE = 0x09;
            const byte ID_LO = 0x0A;
            const byte DEVICE_CATEGORY = 0x0B;
            const byte DEVICE_SUBCATEGORY = 0x0C;
            const byte FIRMWARE_VERSION = 0x0D;

            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x60)
                    .AndReceive(0x02, 0x60, ID_HI, ID_MIDDLE, ID_LO, DEVICE_CATEGORY, DEVICE_SUBCATEGORY, FIRMWARE_VERSION, Constants.ACK);

                var test = buildObjectForTest(scenario.Playback());
                var info = test.GetInfo();

                Assert.AreEqual("08.09.0A", info.DeviceId.ToString());
                Assert.AreEqual(DEVICE_CATEGORY, info.DeviceCategoryCode);
                Assert.AreEqual(DEVICE_SUBCATEGORY, info.DeviceSubcategoryCode);
                Assert.AreEqual(FIRMWARE_VERSION, info.FirmwareVersion);
            }
        }

        [Test]
        public void Can_get_Led()
        {
            var test = buildObjectForTest();
            var led = test.Led;
        }

        [Test]
        public void Can_get_SetButton()
        {
            var test = buildObjectForTest();
            var setButton = test.SetButton;
        }

        private Plm buildObjectForTest()
        {
            var port = MockRepository.GenerateStub<ISerialPortController>();
            return buildObjectForTest(port);
        }

        private Plm buildObjectForTest(ISerialPortController serialPortController)
        {
            return new Plm(serialPortController);
        }
    }
}
