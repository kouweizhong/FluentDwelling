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

namespace SoapBox.FluentDwelling
{
    public delegate void AllLinkingCompletedHandler(object sender, AllLinkingCompletedArgs e);

    public class AllLinkingCompletedArgs : EventArgs
    {
        internal AllLinkingCompletedArgs(byte linkCode, byte group, DeviceId peerId,
            byte deviceCategory, byte deviceSubcategory)
        {
            if (peerId == null) throw new ArgumentNullException("peerId");

            this.AllLinkGroup = group;
            switch (linkCode)
            {
                case 0x00:
                    this.AllLinkingAction = AllLinkingAction.LinkedWithPlmAsSlave;
                    break;
                case 0x01:
                    this.AllLinkingAction = AllLinkingAction.LinkedWithPlmAsMaster;
                    break;
                case 0xFF:
                    this.AllLinkingAction = AllLinkingAction.LinkDeleted;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("linkCode");
            }
            this.PeerId = peerId;
            this.DeviceCategoryCode = deviceCategory;
            this.DeviceSubcategoryCode = deviceSubcategory;
        }

        public byte AllLinkGroup { get; private set; }
        public AllLinkingAction AllLinkingAction { get; private set; }
        public DeviceId PeerId { get; private set; }
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
    }
}
