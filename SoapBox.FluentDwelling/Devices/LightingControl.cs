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
    public class LightingControl : DeviceBase
    {
        internal LightingControl(Plm plm, DeviceId deviceId,
            byte deviceCategory, byte deviceSubcategory)
            : base(plm, deviceId, deviceCategory, deviceSubcategory)
        {
        }

        /// <summary>
        /// Commands the lighting device to turn on immediately
        /// (ignoring ramp rate, if this is a dimmable lighting control).
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOn()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x12, 0xFF);
        }

        /// <summary>
        /// Commands the lighting device to turn off immediately
        /// (ignoring ramp rate, if this is a dimmable lighting control).
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOff()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x14, 0x00);
        }

        /// <summary>
        /// Queries the lighting device for it's current On Level (0 to 255).
        /// For switched lighting controls, this is either off (0) or on (255).
        /// </summary>
        /// <param name="onLevel">Set to on-level if function result is true</param>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetOnLevel(out byte onLevel)
        {
            bool result = false;
            byte onLevelResult = 0;
            base.Plm.exceptionHandler(() =>
            {
                byte[] responseAck = base.Plm.sendStandardLengthMessageAndWait4Response(
                        base.DeviceId, Constants.MSG_FLAGS_DIRECT, 0x19, 0x00);
                byte flags = DeviceMessage.MessageFlags(responseAck);
                result = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
                onLevelResult = DeviceMessage.Command2(responseAck);
            });
            onLevel = onLevelResult;
            return result;
        }
    }
}
