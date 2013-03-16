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
    public delegate void StandardMessageReceivedHandler(object sender, StandardMessageReceivedArgs e);

    public class StandardMessageReceivedArgs : EventArgs
    {
        internal StandardMessageReceivedArgs(DeviceId peerId, byte group, byte flags, byte command1, byte command2)
        {
            if (peerId == null) throw new ArgumentNullException("peerId");
            this.PeerId = peerId;
            this.RawFlags = flags;
            this.Flags = new StandardMessageFlags(flags);
            this.MessageType = (StandardMessageType)(flags & 0xE0);
            this.Command1 = command1;
            this.Command2 = command2;
            switch (this.MessageType)
            {
                case StandardMessageType.GroupBroadcast:
                    this.Group = group;
                    break;
                case StandardMessageType.GroupCleanupDirect:
                    this.Group = command2;
                    break;
                default:
                    this.Group = 0;
                    break;
            }

            this.Description = string.Empty;
            String command2Description;
            switch (this.Command1)
            {
                case 0x10:
                    this.Description = "Ping";
                    break;
                case 0x11:
                    command2Description = string.Empty;
                    if (this.MessageType != StandardMessageType.GroupCleanupDirect
                        && command2 > 0)
                    {
                        command2Description = string.Format(" to level {0}", command2);
                    }
                    this.Description = "Turn On" + command2Description;
                    break;
                case 0x12:
                    command2Description = string.Empty;
                    if (this.MessageType != StandardMessageType.GroupCleanupDirect
                        && command2 > 0)
                    {
                        command2Description = string.Format(" to level {0}", command2);
                    }
                    this.Description = "Fast On" + command2Description;
                    break;
                case 0x13:
                    command2Description = string.Empty;
                    if (this.MessageType != StandardMessageType.GroupCleanupDirect)
                    {
                        command2Description = string.Format(" to level {0}", command2);
                    }
                    this.Description = "Turn Off" + command2Description;
                    break;
                case 0x14:
                    command2Description = string.Empty;
                    if (this.MessageType != StandardMessageType.GroupCleanupDirect)
                    {
                        command2Description = string.Format(" to level {0}", command2);
                    }
                    this.Description = "Fast Off" + command2Description;
                    break;
                case 0x15:
                    this.Description = "Brighten 1 Step";
                    break;
                case 0x16:
                    this.Description = "Dim 1 Step";
                    break;
                case 0x17:
                    if (command2 == 1)
                    {
                        this.Description = "Begin Manual Brightening";
                    }
                    else
                    {
                        this.Description = "Begin Manual Dimming";
                    }
                    break;
                case 0x18:
                    this.Description = "End Manual Brightening/Dimming";
                    break;
                case 0x19:
                    this.Description = "Status Request";
                    break;
            }
        }

        public DeviceId PeerId { get; private set; }
        /// <summary>
        /// Only valid if the Flags.Group bit is set
        /// </summary>
        public byte Group { get; private set; }
        public byte RawFlags { get; private set; }
        public StandardMessageFlags Flags { get; private set; }
        public StandardMessageType MessageType { get; private set; }
        public byte Command1 { get; private set; }
        public byte Command2 { get; private set; }
        /// <summary>
        /// This is a convenience property that only works for common 
        /// message types like Turn On and Turn Off.  It should *not*
        /// be used for control decisions.  Use Command1 and Command2 instead.
        /// </summary>
        public string Description { get; private set; }
    }
}
