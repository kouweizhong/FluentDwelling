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
    class DeviceMessage
    {
        const int MINIMUM_MESSAGE_LENGTH = 11;

        /// <summary>
        /// Used to parse out the sending DeviceId of
        /// a standard or extended length message
        /// </summary>
        /// <param name="message">Standard or extended length message</param>
        public static DeviceId DeviceMessageOriginator(byte[] message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Length < MINIMUM_MESSAGE_LENGTH) throw new ArgumentOutOfRangeException("message");
            return new DeviceId(message[2], message[3], message[4]);
        }

        /// <summary>
        /// Used to parse out the message flags of a 
        /// standard or extended length message.
        /// </summary>
        /// <param name="message">Standard or extended length message</param>
        public static byte MessageFlags(byte[] message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Length < MINIMUM_MESSAGE_LENGTH) throw new ArgumentOutOfRangeException("message");
            return message[8];
        }

        /// <summary>
        /// Used to parse out the Command 1 field of a 
        /// standard or extended length message.
        /// </summary>
        /// <param name="message">Standard or extended length message</param>
        public static byte Command1(byte[] message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Length < MINIMUM_MESSAGE_LENGTH) throw new ArgumentOutOfRangeException("message");
            return message[9];
        }

        /// <summary>
        /// Used to parse out the Command 2 field of a 
        /// standard or extended length message.
        /// </summary>
        /// <param name="message">Standard or extended length message</param>
        public static byte Command2(byte[] message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Length < MINIMUM_MESSAGE_LENGTH) throw new ArgumentOutOfRangeException("message");
            return message[10];
        }
    }
}
