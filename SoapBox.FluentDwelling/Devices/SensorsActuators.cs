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
    public class SensorsActuators : DeviceBase
    {
        internal SensorsActuators(Plm plm, DeviceId deviceId,
            byte deviceCategory, byte deviceSubcategory)
            : base(plm, deviceId, deviceCategory, deviceSubcategory)
        {
        }

        /// <summary>
        /// Commands the I/O device to turn on the given
        /// output.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOnOutput(byte outputNumber)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x45, outputNumber);
        }

        /// <summary>
        /// Commands the I/O device to turn off the given
        /// output.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOffOutput(byte outputNumber)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x46, outputNumber);
        }

        /// <summary>
        /// Writes the given byte value to the output port, which affects
        /// up to 8 output bits at once.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool WriteByteToOutputPort(byte outputByteValue)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x48, outputByteValue);
        }

        /// <summary>
        /// Commands the I/O device to read the input port value, which
        /// is up to 8 input bits.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetInputPort(out byte inputPortValue)
        {
            bool result = false;
            byte retrievedInputPort = 0;
            base.Plm.exceptionHandler(() =>
            {
                byte[] responseAck = base.Plm.sendStandardLengthMessageAndWait4Response(
                        base.DeviceId, Constants.MSG_FLAGS_DIRECT, 0x49, 0x00);
                byte flags = DeviceMessage.MessageFlags(responseAck);
                result = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
                retrievedInputPort = DeviceMessage.Command2(responseAck);
            });
            inputPortValue = retrievedInputPort;
            return result;
        }

        /// <summary>
        /// Commands the I/O device to read the given sensor input, and return
        /// the 8 bit value.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetSensorValue(byte sensorNumber, out byte sensorValue)
        {
            bool result = false;
            byte retrievedSensorValue = 0;
            base.Plm.exceptionHandler(() =>
            {
                byte[] responseAck = base.Plm.sendStandardLengthMessageAndWait4Response(
                        base.DeviceId, Constants.MSG_FLAGS_DIRECT, 0x4A, sensorNumber);
                byte flags = DeviceMessage.MessageFlags(responseAck);
                result = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
                retrievedSensorValue = DeviceMessage.Command2(responseAck);
            });
            sensorValue = retrievedSensorValue;
            return result;
        }


    }
}
