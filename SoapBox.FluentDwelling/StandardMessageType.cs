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
    public enum StandardMessageType
    {
        Broadcast = Constants.MSG_FLAGS_BROADCAST,
        Direct = Constants.MSG_FLAGS_DIRECT,
        AckOfDirect = Constants.MSG_FLAGS_DIRECT_ACK,
        NackOfDirect = Constants.MSG_FLAGS_DIRECT_NACK,
        GroupBroadcast = Constants.MSG_FLAGS_GROUP_BROADCAST,
        GroupCleanupDirect = Constants.MSG_FLAGS_GROUP_CLEANUP,
        AckOfGroupCleanupDirect = Constants.MSG_FLAGS_GROUP_CLEANUP_ACK,
        NackOfGroupCleanupDirect = Constants.MSG_FLAGS_GROUP_CLEANUP_NACK
    }
}
