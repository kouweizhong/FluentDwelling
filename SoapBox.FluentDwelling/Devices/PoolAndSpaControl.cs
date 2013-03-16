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
    public class PoolAndSpaControl : DeviceBase
    {
        internal PoolAndSpaControl(Plm plm, DeviceId deviceId,
            byte deviceCategory, byte deviceSubcategory)
            : base(plm, deviceId, deviceCategory, deviceSubcategory)
        {
        }

        /// <summary>
        /// Commands the pool/spa control to turn on the given
        /// device.  "All" is not a valid option.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOnDevice(PoolAndSpaDevice device)
        {
            return TurnOnDevice((byte)device);
        }

        /// <summary>
        /// Commands the pool/spa control to turn on the given
        /// device number.  "0" is not a valid option.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOnDevice(byte deviceNumber)
        {
            if (deviceNumber == 0) throw new ArgumentOutOfRangeException("deviceNumber");
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x50, deviceNumber);
        }

        /// <summary>
        /// Commands the pool/spa control to turn off the given
        /// device. 
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOffDevice(PoolAndSpaDevice device)
        {
            return TurnOffDevice((byte)device);
        }

        /// <summary>
        /// Commands the pool/spa control to turn off the given
        /// device number. "0" == turn off all devices.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOffDevice(byte deviceNumber)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x51, deviceNumber);
        }

        /// <summary>
        /// Commands the pool/spa control to raise the temperature
        /// setpoint by the given value * 0.5.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool RaiseTemperatureSettingBy(byte degreesTimesTwo)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x52, degreesTimesTwo);
        }

        /// <summary>
        /// Commands the pool/spa control to lower the temperature
        /// setpoint by the given value * 0.5.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool LowerTemperatureSettingBy(byte degreesTimesTwo)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x53, degreesTimesTwo);
        }

        /// <summary>
        /// Commands the pool/spa device to load initialization values.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool LoadInitializationValues()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x54, 0x00);
        }

        /// <summary>
        /// Commands the pool/spa device to Load EEPROM parameters from RAM.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool LoadEepromFromRam()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x54, 0x01);
        }

        /// <summary>
        /// Commands the pool/spa device to return the thermostat mode (Pool or Spa).
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetThermostatMode(out PoolAndSpaThermostatMode thermostatMode)
        {
            bool result = false;
            byte retrievedThermostatMode = 0;
            base.Plm.exceptionHandler(() =>
            {
                byte[] responseAck = base.Plm.sendStandardLengthMessageAndWait4Response(
                        base.DeviceId, Constants.MSG_FLAGS_DIRECT, 0x54, 0x02);
                byte flags = DeviceMessage.MessageFlags(responseAck);
                result = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
                retrievedThermostatMode = DeviceMessage.Command2(responseAck);
            });
            switch(retrievedThermostatMode)
            {
                case 1:
                    thermostatMode = PoolAndSpaThermostatMode.Spa;
                    break;
                default:
                    thermostatMode = PoolAndSpaThermostatMode.Pool;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Tries to retrieve the ambient temperature from the Pool/Spa controller
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetAmbientTemperature(out byte ambientTemperature)
        {
            return getByte(out ambientTemperature, 0x03);
        }

        /// <summary>
        /// Tries to retrieve the water temperature from the Pool/Spa controller
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetWaterTemperature(out byte waterTemperature)
        {
            return getByte(out waterTemperature, 0x04);
        }

        /// <summary>
        /// Tries to retrieve the pH from the Pool/Spa controller
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetPH(out byte pH)
        {
            return getByte(out pH, 0x05);
        }

        private bool getByte(out byte readFromDevice, byte command2)
        {
            bool result = false;
            byte retrievedByteFromDevice = 0;
            base.Plm.exceptionHandler(() =>
            {
                byte[] responseAck = base.Plm.sendStandardLengthMessageAndWait4Response(
                        base.DeviceId, Constants.MSG_FLAGS_DIRECT, 0x54, command2);
                byte flags = DeviceMessage.MessageFlags(responseAck);
                result = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
                retrievedByteFromDevice = DeviceMessage.Command2(responseAck);
            });
            readFromDevice = retrievedByteFromDevice;
            return result;
        }

    }
}
