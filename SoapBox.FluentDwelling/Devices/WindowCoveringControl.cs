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
    public class WindowCoveringControl : DeviceBase
    {
        internal WindowCoveringControl(Plm plm, DeviceId deviceId,
            byte deviceCategory, byte deviceSubcategory)
            : base(plm, deviceId, deviceCategory, deviceSubcategory)
        {
        }

        /// <summary>
        /// Commands the window covering to open.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool Open()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x60, 0x00);
        }

        /// <summary>
        /// Commands the window covering to close.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool Close()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x60, 0x01);
        }

        /// <summary>
        /// Commands the window covering to stop opening or closing.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool Stop()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x60, 0x02);
        }

        /// <summary>
        /// Puts the device in program mode?  Not really sure...
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool Program()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x60, 0x03);
        }

        /// <summary>
        /// Moves the window covering to the given position, where
        /// 0x00 is closed and 0xFF is open
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool MoveToPosition(byte position)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x61, position);
        }


    }
}
