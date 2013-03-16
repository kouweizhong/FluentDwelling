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
    internal interface ISerialPortController : IDisposable
    {
        /// <summary>
        /// Sends a byte array via a serial port, and then
        /// waits for a response of a given length.
        /// </summary>
        /// <param name="send">Bytes to send</param>
        /// <param name="receiveLength">Bytes to receive</param>
        /// <returns>Received bytes (will be of length specified)</returns>
        byte[] SendReceive(byte[] send, int receiveLength);

        /// <summary>
        /// Looks for incoming messages in the buffer and
        /// sets incomingMessage to the first one if there is one.
        /// Returns true if there is.
        /// </summary>
        /// <param name="incomingMessage">Where to store incoming message</param>
        /// <returns>true if stored</returns>
        bool TryGetIncomingMessages(out byte[] incomingMessage);

        /// <summary>
        /// Blocks waiting for an incoming message of the given
        /// message type.  Will eventually throw a TimeoutException.
        /// Returns the received message.
        /// </summary>
        byte[] GetIncomingMessageOfType(params byte[] messageTypes);
    }
}
