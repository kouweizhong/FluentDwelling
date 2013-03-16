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
using Rhino.Mocks;

namespace SoapBox.FluentDwelling.Test.Devices
{
    [TestFixture]
    public class TestDeviceFactory
    {
        [Test]
        public void Test_device_category_to_Type()
        {
            testCategory<GeneralizedController>(0x00);
            testCategory<DimmableLightingControl>(0x01);
            testCategory<SwitchedLightingControl>(0x02);
            testCategory<NetworkBridge>(0x03);
            testCategory<IrrigationControl>(0x04);
            testCategory<ClimateControl>(0x05);
            testCategory<PoolAndSpaControl>(0x06);
            testCategory<SensorsActuators>(0x07);
            testCategory<HomeEntertainmentControl>(0x08);
            testCategory<EnergyManagement>(0x09);
            testCategory<BuiltInApplianceControl>(0x0A);
            testCategory<PlumbingControl>(0x0B);
            testCategory<CommunicationControl>(0x0C);
            testCategory<ComputerControl>(0x0D);
            testCategory<WindowCoveringControl>(0x0E);
            testCategory<AccessControl>(0x0F);
            testCategory<SecurityHealthOrSafetyControl>(0x10);
            testCategory<SurveillanceControl>(0x11);
            testCategory<AutomotiveControl>(0x12);
            testCategory<PetCareControl>(0x13);
            testCategory<Toy>(0x14);
            testCategory<TimekeepingControl>(0x15);
            testCategory<HolidayControl>(0x16);
            testCategory<DeviceBase>(0xFF);
        }

        private void testCategory<T>(byte deviceCategory)
            where T : DeviceBase
        {
            testCategoryAndSubcategory<T>(deviceCategory, 0x00);
        }

        private void testCategoryAndSubcategory<T>(byte deviceCategory, byte deviceSubcategory)
            where T : DeviceBase
        {
            var serialPortController = MockRepository.GenerateStub<ISerialPortController>();
            var device = TestDeviceHelper.BuildDeviceForTest<T>(deviceCategory, deviceSubcategory, serialPortController);
            Assert.IsInstanceOf<T>(device);
        }
    }
}
