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

namespace SoapBox.FluentDwelling.Devices
{
    internal static class DeviceFactory
    {
        public static DeviceBase BuildDevice(Plm plm, byte[] idRequestResponse)
        {
            if (idRequestResponse == null) throw new ArgumentNullException("idRequestResponse");

            var deviceId = DeviceMessage.DeviceMessageOriginator(idRequestResponse);
            byte category = deviceCategory(idRequestResponse);
            byte subcategory = deviceSubcategory(idRequestResponse);
            switch (category)
            {
                case 0x00:
                    return new GeneralizedController(plm, deviceId, category, subcategory);
                case 0x01:
                    return new DimmableLightingControl(plm, deviceId, category, subcategory);
                case 0x02:
                    return new SwitchedLightingControl(plm, deviceId, category, subcategory);
                case 0x03:
                    return new NetworkBridge(plm, deviceId, category, subcategory);
                case 0x04:
                    return new IrrigationControl(plm, deviceId, category, subcategory);
                case 0x05:
                    return new ClimateControl(plm, deviceId, category, subcategory);
                case 0x06:
                    return new PoolAndSpaControl(plm, deviceId, category, subcategory);
                case 0x07:
                    return new SensorsActuators(plm, deviceId, category, subcategory);
                case 0x08:
                    return new HomeEntertainmentControl(plm, deviceId, category, subcategory);
                case 0x09:
                    return new EnergyManagement(plm, deviceId, category, subcategory);
                case 0x0A:
                    return new BuiltInApplianceControl(plm, deviceId, category, subcategory);
                case 0x0B:
                    return new PlumbingControl(plm, deviceId, category, subcategory);
                case 0x0C:
                    return new CommunicationControl(plm, deviceId, category, subcategory);
                case 0x0D:
                    return new ComputerControl(plm, deviceId, category, subcategory);
                case 0x0E:
                    return new WindowCoveringControl(plm, deviceId, category, subcategory);
                case 0x0F:
                    return new AccessControl(plm, deviceId, category, subcategory);
                case 0x10:
                    return new SecurityHealthOrSafetyControl(plm, deviceId, category, subcategory);
                case 0x11:
                    return new SurveillanceControl(plm, deviceId, category, subcategory);
                case 0x12:
                    return new AutomotiveControl(plm, deviceId, category, subcategory);
                case 0x13:
                    return new PetCareControl(plm, deviceId, category, subcategory);
                case 0x14:
                    return new Toy(plm, deviceId, category, subcategory);
                case 0x15:
                    return new TimekeepingControl(plm, deviceId, category, subcategory);
                case 0x16:
                    return new HolidayControl(plm, deviceId, category, subcategory);
                default:
                    return new DeviceBase(plm, deviceId, category, subcategory);
            }
        }

        private static byte deviceCategory(byte[] idRequestResponse)
        {
            if (idRequestResponse.Length >= 6)
            {
                return idRequestResponse[5];
            }
            throw new ArgumentOutOfRangeException("idRequestResponse");
        }

        private static byte deviceSubcategory(byte[] idRequestResponse)
        {
            if (idRequestResponse.Length >= 7)
            {
                return idRequestResponse[6];
            }
            throw new ArgumentOutOfRangeException("idRequestResponse");
        }
    }
}
