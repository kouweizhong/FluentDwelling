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
using SoapBox.FluentDwelling.Devices;

namespace SoapBox.FluentDwelling
{
    public class PlmInfo : DeviceBase
    {
        /// <summary>
        /// Creates an object with all values zero
        /// </summary>
        internal PlmInfo(Plm plm) : base(plm, new DeviceId(0,0,0), 0, 0) { }

        /// <summary>
        /// Creates an object based on the raw data
        /// received from Get IM Info call
        /// </summary>
        /// <param name="received">Should be 9 bytes</param>
        internal PlmInfo(Plm plm, byte[] received)
            : base(plm, new DeviceId(received[2], received[3], received[4]), received[5], received[6])
        {
            if (received == null) throw new ArgumentNullException("received");
            if (received.Length != 9) throw new ArgumentOutOfRangeException("received");
            this.FirmwareVersion = received[7];
        }

        public byte FirmwareVersion { get; private set; }

    }
}
