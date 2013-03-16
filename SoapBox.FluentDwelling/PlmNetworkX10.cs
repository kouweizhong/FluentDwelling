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
using System.Threading;

namespace SoapBox.FluentDwelling
{
    public enum X10Command
    {
        AllLightsOff = 0x06,
        StatusIsOff = 0x0E,
        On = 0x02,
        PresetDim1 = 0x0A,
        AllLightsOn = 0x01,
        HailAcknowledge = 0x09,
        Bright = 0x05,
        StatusIsOn = 0x0D,
        ExtendedCode = 0x07,
        StatusRequest = 0x0F,
        Off = 0x03,
        PresetDim2 = 0x0B,
        AllUnitsOff = 0x00,
        HailRequest = 0x08,
        Dim = 0x04,
        ExtendedData = 0x0C
    }

    public class PlmNetworkX10
    {
        const int INTERMESSAGE_WAIT_MS = 500;

        private readonly Plm plm;

        internal PlmNetworkX10(Plm plm)
        {
            if (plm == null) throw new ArgumentNullException("plm");
            this.plm = plm;
        }

        /// <summary>
        /// This event is fired when an X10 unit code
        /// like "A2" is received over the network.
        /// </summary>
        public event X10UnitAddressedHandler UnitAddressed;

        private void fireUnitAddressedEvent(byte x10Code)
        {
            var evt = UnitAddressed;
            if (evt != null)
            {
                evt(this, new X10UnitAddressedArgs(x10Code));
            }
        }

        /// <summary>
        /// This event is fired when an X10 command
        /// like "On" is received over the network.
        /// </summary>
        public event X10CommandReceivedHandler CommandReceived;

        private void fireCommandReceivedEvent(byte x10Code)
        {
            var evt = CommandReceived;
            if (evt != null)
            {
                evt(this, new X10CommandReceivedArgs(x10Code));
            }
        }

        internal void x10MessageReceived(byte x10Code, byte x10Flag)
        {
            switch (x10Flag)
            {
                case 0x00:
                    fireUnitAddressedEvent(x10Code);
                    break;
                case 0x80:
                    fireCommandReceivedEvent(x10Code);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("x10Flag");
            }
        }

        public HouseContext House(string houseCode)
        {
            if (houseCode == null) throw new ArgumentNullException("houseCode");
            var upperHouseCode = houseCode.ToUpper();
            if (!Constants.X10HouseCodeLookup.ContainsKey(upperHouseCode)) throw new ArgumentOutOfRangeException("houseCode");
            return new HouseContext(this.plm, upperHouseCode);
        }

        public class HouseContext
        {
            private readonly Plm plm;
            private readonly string houseCode;
            private DateTime lastSendTime = DateTime.MinValue; // have to delay sending between messages

            internal HouseContext(Plm plm, string houseCode)
            {
                this.plm = plm;
                this.houseCode = houseCode;
            }

            public HouseContext Unit(byte unitCode)
            {
                if(!Constants.X10UnitCodeLookup.ContainsKey(unitCode)) throw new ArgumentOutOfRangeException("unitCode");
                delaySendingIfNecessary();
                byte x10Code = (byte)(
                    Constants.X10HouseCodeLookup[this.houseCode] |
                    Constants.X10UnitCodeLookup[unitCode]);
                this.plm.exceptionHandler(() =>
                    {
                        this.plm.sendCommandWithEchoAndAck(0x02, 0x63, x10Code, 0x00);
                        updateLastSendTime();
                    });
                return this;
            }

            public HouseContext Command(X10Command command)
            {
                delaySendingIfNecessary();
                byte x10Code = (byte)(
                    Constants.X10HouseCodeLookup[this.houseCode] |
                    (byte)command);
                this.plm.exceptionHandler(() =>
                {
                    this.plm.sendCommandWithEchoAndAck(0x02, 0x63, x10Code, 0x80);
                    updateLastSendTime();
                });
                return this;
            }

            private void delaySendingIfNecessary()
            {
                double elapsed = DateTime.Now.Subtract(this.lastSendTime).TotalMilliseconds;
                if(elapsed >= INTERMESSAGE_WAIT_MS) return;
                int diff = INTERMESSAGE_WAIT_MS - (int)elapsed;
                Thread.Sleep(diff);
            }

            private void updateLastSendTime()
            {
                this.lastSendTime = DateTime.Now;
            }
        }
    }
}
