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
    public class DimmableLightingControl : LightingControl
    {
        internal DimmableLightingControl(Plm plm, DeviceId deviceId,
            byte deviceCategory, byte deviceSubcategory)
            : base(plm, deviceId, deviceCategory, deviceSubcategory)
        {
        }

        /// <summary>
        /// Commands the lighting device to turn on immediately,
        /// ignoring ramp rate, to the set level.
        /// </summary>
        /// <param name="onLevel">Light level (0-255)</param>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOn(byte onLevel)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x12, onLevel);
        }

        /// <summary>
        /// Commands the lighting device to turn on using the saved
        /// ramp rate, to the set level.
        /// </summary>
        /// <param name="onLevel">Light level (0-255)</param>
        /// <returns>True if the device responds with an ACK</returns>
        public bool RampOn(byte onLevel)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x11, onLevel);
        }

        /// <summary>
        /// Commands the lighting device to turn off using the saved
        /// ramp rate.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool RampOff()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x13, 0x00);
        }

        /// <summary>
        /// Commands the lighting device to brighten one level.  There
        /// are 32 brightness levels.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool BrightenOneStep()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x15, 0x00);
        }

        /// <summary>
        /// Commands the lighting device to dim one level.  There
        /// are 32 brightness levels.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool DimOneStep()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x16, 0x00);
        }

        /// <summary>
        /// Commands the lighting device to begin ramping up the on-level.
        /// Use StopBrighteningOrDimming to cancel.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool BeginBrightening()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x17, 0x01);
        }

        /// <summary>
        /// Commands the lighting device to begin ramping down the on-level.
        /// Use StopBrighteningOrDimming to cancel.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool BeginDimming()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x17, 0x00);
        }

        /// <summary>
        /// Commands the lighting device to stop the ramping of the 
        /// on-level that was started with BeginBrightening or BeginDimming
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool StopBrighteningOrDimming()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x18, 0x00);
        }


    }
}
