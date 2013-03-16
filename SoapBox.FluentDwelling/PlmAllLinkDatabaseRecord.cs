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
    public class PlmAllLinkDatabaseRecord
    {
        private readonly byte[] record;

        internal PlmAllLinkDatabaseRecord(byte[] record)
        {
            if (record == null) throw new ArgumentNullException("record");
            if (record.Length != 10) throw new ArgumentOutOfRangeException("record");
            this.record = record;
            this.InUse = (record[2] & Constants.ALL_LINK_DB_FLAGS_RECORD_IN_USE) > 0;
            this.PlmIsMaster = (record[2] & Constants.ALL_LINK_DB_FLAGS_PLM_IS_MASTER) > 0;
            this.AllLinkGroup = record[3];
            this.DeviceId = new DeviceId(record[4], record[5], record[6]);
        }

        public bool InUse { get; private set; }
        public bool PlmIsMaster { get; private set; }
        public byte AllLinkGroup { get; private set; }
        public DeviceId DeviceId { get; private set; }
        public byte LinkSpecificData(int index)
        {
            if (index < 0 || index > 2) throw new ArgumentOutOfRangeException("index");
            return record[7 + index];
        }
    }
}
