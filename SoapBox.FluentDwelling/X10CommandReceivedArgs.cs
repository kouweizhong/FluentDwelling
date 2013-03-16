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
    public delegate void X10CommandReceivedHandler(object sender, X10CommandReceivedArgs e);

    public class X10CommandReceivedArgs : EventArgs
    {
        internal X10CommandReceivedArgs(byte x10Code)
        {
            byte houseCodePart = (byte)(x10Code & 0xF0);
            if(!Constants.X10HouseCodeReverseLookup.ContainsKey(houseCodePart))
            {
                throw new ArgumentOutOfRangeException("x10Code");
            }
            this.HouseCode = Constants.X10HouseCodeReverseLookup[houseCodePart];

            byte commandPart = (byte)(x10Code & 0x0F);
            if (Enum.IsDefined(typeof(X10Command), (int)commandPart))
            {
                this.Command = (X10Command)commandPart;
            }
            else
            {
                throw new ArgumentOutOfRangeException("x10Code");
            }
        }

        public string HouseCode { get; private set; }
        public X10Command Command { get; private set; }
    }
}
