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
    public class TestPlmConfiguration
    {
        [Test]
        public void Decodes_configuration()
        {
            PlmConfiguration test;

            test = new PlmConfiguration(Constants.CONFIG_DISABLE_AUTO_LINKING);
            Assert.IsTrue(test.AutoLinkingDisabled);
            Assert.IsFalse(test.MonitorMode);
            Assert.IsFalse(test.ManualLedControl);
            Assert.IsFalse(test.Rs232Deadman);

            test = new PlmConfiguration(Constants.CONFIG_MONITOR_MODE);
            Assert.IsFalse(test.AutoLinkingDisabled);
            Assert.IsTrue(test.MonitorMode);
            Assert.IsFalse(test.ManualLedControl);
            Assert.IsFalse(test.Rs232Deadman);

            test = new PlmConfiguration(Constants.CONFIG_MANUAL_LED_CONTROL);
            Assert.IsFalse(test.AutoLinkingDisabled);
            Assert.IsFalse(test.MonitorMode);
            Assert.IsTrue(test.ManualLedControl);
            Assert.IsFalse(test.Rs232Deadman);

            test = new PlmConfiguration(Constants.CONFIG_DISABLE_RS232_DEADMAN);
            Assert.IsFalse(test.AutoLinkingDisabled);
            Assert.IsFalse(test.MonitorMode);
            Assert.IsFalse(test.ManualLedControl);
            Assert.IsTrue(test.Rs232Deadman);


            test = new PlmConfiguration(Constants.CONFIG_DISABLE_AUTO_LINKING | Constants.CONFIG_DISABLE_RS232_DEADMAN);
            Assert.IsTrue(test.AutoLinkingDisabled);
            Assert.IsFalse(test.MonitorMode);
            Assert.IsFalse(test.ManualLedControl);
            Assert.IsTrue(test.Rs232Deadman);
        }

    }
}
