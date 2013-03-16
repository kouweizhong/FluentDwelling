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
    internal class IncomingMessageBuffer
    {
        private readonly IList<IncomingMessage> messages = new List<IncomingMessage>();
        private readonly List<byte> buffer = new List<byte>();

        public void AddBytes(params byte[] newBytes)
        {
            foreach (var b in newBytes)
            {
                this.buffer.Add(b);
            }
            lookForMessages();
        }

        private void lookForMessages()
        {
            discardGarbageDataFromBeginningOfBuffer();
            while (canDetermineMessageLength()
                && this.buffer.Count >= messageLength())
            {
                int length = messageLength();
                byte[] newMessage = this.buffer.Take(length).ToArray();
                this.buffer.RemoveRange(0, length);
                this.messages.Add(new IncomingMessage(newMessage));
                discardGarbageDataFromBeginningOfBuffer();
            }
        }

        private void discardGarbageDataFromBeginningOfBuffer()
        {
            while (this.buffer.Count >= 2 && !startOfMessageAtStartOfBuffer())
            {
                this.buffer.RemoveAt(0); // discard any garbage data
            }
        }

        private bool startOfMessageAtStartOfBuffer()
        {
            return
                this.buffer.Count >= 2
                && this.buffer[0] == 0x02
                && recognizedMessageType(this.buffer[1]);
        }

        private bool recognizedMessageType(byte messageType)
        {
            return Constants.IncomingMessageLengthLookup.ContainsKey(messageType);
        }

        private bool canDetermineMessageLength()
        {
            return
                startOfMessageAtStartOfBuffer() &&
                (this.buffer[1] != 0x62 || this.buffer.Count >= 6);
        }

        private int messageLength()
        {
            byte messageType = this.buffer[1];
            if (messageType == 0x62)
            {
                // special case - need to look at bit 4 of 6th byte
                byte sixthByte = this.buffer[5];
                bool bit4 = (sixthByte & 0x10) > 0;
                return bit4 ? 23 : 9;
            }
            else
            {
                return Constants.IncomingMessageLengthLookup[messageType];
            }
        }

        public bool TryGetMessageByType(out IncomingMessage message, params byte[] messageTypes)
        {
            message =
                (from m in this.messages
                 where messageTypes.Contains(m.MessageType)
                 select m).FirstOrDefault();
            bool result = message != null;
            if (result)
            {
                this.messages.Remove(message);
            } 
            return result;
        }

        public bool TryGetIncomingMessage(out IncomingMessage message)
        {
            message =
                (from m in this.messages
                 select m).FirstOrDefault();
            bool result = message != null;
            if (result)
            {
                this.messages.Remove(message);
            }
            return result;
        }

        public void Clear()
        {
            this.buffer.Clear();
            this.messages.Clear();
        }
    }
}
