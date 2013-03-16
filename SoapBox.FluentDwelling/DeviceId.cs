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
using System.Text.RegularExpressions;

namespace SoapBox.FluentDwelling
{
    public class DeviceId
    {
        public DeviceId(string dottedHexId)
            : this(toIdBytes(dottedHexId)) { }

        internal DeviceId(params byte[] idBytes)
        {
            this.IdHi = idBytes[0];
            this.IdMiddle = idBytes[1];
            this.IdLo = idBytes[2];
        }

        public byte IdHi { get; private set; }
        public byte IdMiddle { get; private set; }
        public byte IdLo { get; private set; }

        private static byte[] toIdBytes(string dottedHexId)
        {
            if (dottedHexId == null) throw new ArgumentNullException("dottedHexId");
            if (!isValidIdString(dottedHexId)) throw new ArgumentOutOfRangeException("dottedHexId");
            var id = dottedHexId.ToLower();
            var idBytes = new byte[3];
            idBytes[0] = Convert.ToByte(id.Substring(0, 2), 16);
            idBytes[1] = Convert.ToByte(id.Substring(3, 2), 16);
            idBytes[2] = Convert.ToByte(id.Substring(6, 2), 16);
            return idBytes;
        }

        private static bool isValidIdString(string dottedHexId)
        {
            const string START = "^";
            const string END = "$";
            const string HEX_NIBBLE = "[0-9a-fA-F]";
            const string HEX_BYTE = HEX_NIBBLE + HEX_NIBBLE;
            const string DOT = "\\.";
            const string ID_PATTERN = START + HEX_BYTE + DOT + HEX_BYTE + DOT + HEX_BYTE + END;
            var pattern = new Regex(ID_PATTERN);
            return pattern.IsMatch(dottedHexId, 0);
        }

        public override string ToString()
        {
            return
                this.IdHi.ToString("X2") + "." +
                this.IdMiddle.ToString("X2") + "." +
                this.IdLo.ToString("X2");
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var otherId = obj as DeviceId;
            return Equals(otherId);
        }

        public bool Equals(DeviceId otherId)
        {
            if ((object)otherId == null) return false;
            return otherId.ToString() == this.ToString();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static bool operator ==(DeviceId a, DeviceId b)
        {
            if (object.ReferenceEquals(a, b)) return true;

            if (((object)a == null) || ((object)b == null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(DeviceId a, DeviceId b)
        {
            return !(a == b);
        }
    }
}
