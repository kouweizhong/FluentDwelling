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
    public delegate void X10UnitAddressedHandler(object sender, X10UnitAddressedArgs e);

    public class X10UnitAddressedArgs : EventArgs
    {
        internal X10UnitAddressedArgs(byte x10Code)
        {
            byte houseCodePart = (byte)(x10Code & 0xF0);
            byte unitCodePart = (byte)(x10Code & 0x0F);
            if (!Constants.X10HouseCodeReverseLookup.ContainsKey(houseCodePart) ||
                !Constants.X10UnitCodeReverseLookup.ContainsKey(unitCodePart))
            {
                throw new ArgumentOutOfRangeException("x10Code");
            }
            this.HouseCode = Constants.X10HouseCodeReverseLookup[houseCodePart];
            this.UnitCode = Constants.X10UnitCodeReverseLookup[unitCodePart];
        }

        public string HouseCode { get; private set; }
        public byte UnitCode { get; private set; }
    }
}
