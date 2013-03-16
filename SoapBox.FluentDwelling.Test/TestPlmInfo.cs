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
    public class TestPlmInfo
    {
        [Test]
        public void Decodes_device_category()
        {
            testDeviceCategory(0x00, "Generalized Controllers");
            testDeviceCategory(0x01, "Dimmable Lighting Control");
            testDeviceCategory(0x02, "Switched Lighting Control");
            testDeviceCategory(0x03, "Network Bridges");
            testDeviceCategory(0x04, "Irrigation Control");
            testDeviceCategory(0x05, "Climate Control");
            testDeviceCategory(0x06, "Pool and Spa Control");
            testDeviceCategory(0x07, "Sensors and Actuators");
            testDeviceCategory(0x08, "Home Entertainment");
            testDeviceCategory(0x09, "Energy Management");
            testDeviceCategory(0x0A, "Built-In Appliance Control");
            testDeviceCategory(0x0B, "Plumbing");
            testDeviceCategory(0x0C, "Communication");
            testDeviceCategory(0x0D, "Computer Control");
            testDeviceCategory(0x0E, "Window Coverings");
            testDeviceCategory(0x0F, "Access Control");
            testDeviceCategory(0x10, "Security, Health, Safety");
            testDeviceCategory(0x11, "Surveillance");
            testDeviceCategory(0x12, "Automotive");
            testDeviceCategory(0x13, "Pet Care");
            testDeviceCategory(0x14, "Toys");
            testDeviceCategory(0x15, "Timekeeping");
            testDeviceCategory(0x16, "Holiday");
            testDeviceCategory(0xFF, "Unassigned");
        }

        private void testDeviceCategory(byte deviceCategory, string name)
        {
            var serialPortController = MockRepository.GenerateStub<ISerialPortController>();
            var plm = new Plm(serialPortController);
            var test = buildObjectForTest(plm, 0, 0, 0, deviceCategory, 0, 0);
            Assert.AreEqual(name, test.DeviceCategory);
        }

        private static PlmInfo buildObjectForTest(Plm plm, byte idHi, byte idMiddle, byte idLo,
            byte deviceCategory, byte deviceSubcategory, byte firmwareRevision)
        {
            return new PlmInfo(plm, new byte[] { 0, 0, idHi, idMiddle, idLo, 
                deviceCategory, deviceSubcategory, firmwareRevision, 0 });
        }
    }
}
