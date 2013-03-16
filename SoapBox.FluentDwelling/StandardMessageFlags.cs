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
    public class StandardMessageFlags
    {
        internal StandardMessageFlags(byte flags)
        {
            this.Broadcast = (flags & Constants.MSG_FLAGS_BROADCAST) > 0;
            this.Group = (flags & Constants.MSG_FLAGS_GROUP) > 0;
            this.Acknowledge = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
            this.Extended = (flags & Constants.MSG_FLAGS_EXTENDED) > 0;
            this.HopsLeft = (flags & 0x0C) / 4;
            this.MaxHops = flags & 0x03;
        }

        public bool Broadcast { get; private set; }
        public bool Group { get; private set; }
        public bool Acknowledge { get; private set; }
        public bool Extended { get; private set; }
        public int HopsLeft { get; private set; }
        public int MaxHops { get; private set; }
    }
}
