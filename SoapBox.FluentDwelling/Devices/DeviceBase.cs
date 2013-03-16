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
    public class DeviceBase
    {
        private readonly Plm plm;
        private readonly DeviceId deviceId;

        internal DeviceBase(Plm plm, DeviceId deviceId, 
            byte deviceCategory, byte deviceSubcategory)
        {
            this.plm = plm;
            this.deviceId = deviceId;
            this.DeviceCategoryCode = deviceCategory;
            this.DeviceSubcategoryCode = deviceSubcategory;
        }

        public Plm Plm { get { return this.plm; } }
        public DeviceId DeviceId { get { return this.deviceId; } }
        public byte DeviceCategoryCode { get; private set; }
        public byte DeviceSubcategoryCode { get; private set; }

        /// <summary>
        /// Returns the text description of the device category
        /// based on the .DeviceCategoryCode value.
        /// </summary>
        public string DeviceCategory
        {
            get
            {
                if (Constants.DeviceCategoryLookup.ContainsKey(this.DeviceCategoryCode))
                    return Constants.DeviceCategoryLookup[this.DeviceCategoryCode];
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns the text description of the device subcategory
        /// based on the .DeviceCategoryCode and .DeviceSubcategoryCode values.
        /// </summary>
        public string DeviceSubcategory
        {
            get
            {
                string result = string.Empty;
                if (Constants.DeviceSubcategoryLookup.ContainsKey(this.DeviceCategoryCode))
                {
                    var innerLookup = Constants.DeviceSubcategoryLookup[this.DeviceCategoryCode];
                    if (innerLookup.ContainsKey(this.DeviceSubcategoryCode))
                    {
                        result = innerLookup[this.DeviceSubcategoryCode];
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// Attempts to send a ping message to the device, and 
        /// returns true if the device responded with an ACK message.
        /// </summary>
        public bool Ping()
        {
            return this.Plm.Network
                .SendStandardCommandToAddress(this.DeviceId, 0x0F, 0x00);
        }
    }
}
