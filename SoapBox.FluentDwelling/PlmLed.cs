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
    public class PlmLed
    {
        private readonly Plm plm;

        internal PlmLed(Plm plm)
        {
            if (plm == null) throw new ArgumentNullException("plm");
            this.plm = plm;
        }

        /// <summary>
        /// Modifies the PLM configuration to set bit 5
        /// ("Disables automatic LED operation by the IM")
        /// so that the PLM LED will now respond to TurnOn()
        /// and TurnOff() commands.
        /// </summary>
        public PlmLed EnableManualControl()
        {
            this.plm.exceptionHandler(() =>
            {
                byte configuration = this.plm.getIMConfiguration();
                byte newConfiguration = (byte)(configuration | Constants.CONFIG_MANUAL_LED_CONTROL);
                this.plm.setIMConfiguration(newConfiguration);
            });
            return this;
        }

        /// <summary>
        /// Modifies the PLM configuration to clear bit 5
        /// ("Disables automatic LED operation by the IM")
        /// so that the PLM LED will now be automatically
        /// controlled by the PLM, and won't respond to
        /// TurnOn() and TurnOff() commands.
        /// </summary>
        public PlmLed DisableManualControl()
        {
            this.plm.exceptionHandler(() =>
            {
                byte configuration = this.plm.getIMConfiguration();
                byte newConfiguration = (byte)(configuration & 255 - Constants.CONFIG_MANUAL_LED_CONTROL);
                this.plm.setIMConfiguration(newConfiguration);
            });
            return this;
        }

        /// <summary>
        /// Sends the LED On command to the PLM.  This will
        /// turn on the LED, only if manual control of the
        /// PLM LED has been enabled (see EnableManualControl())
        /// </summary>
        public PlmLed TurnOn()
        {
            this.plm.exceptionHandler(() =>
            {
                this.plm.sendCommandWithEchoAndAck(0x02, 0x6D);
            });
            return this;
        }

        /// <summary>
        /// Sends the LED Off command to the PLM.  This will
        /// turn off the LED, only if manual control of the
        /// PLM LED has been enabled (see EnableManualControl())
        /// </summary>
        public PlmLed TurnOff()
        {
            this.plm.exceptionHandler(() =>
            {
                this.plm.sendCommandWithEchoAndAck(0x02, 0x6E);
            });
            return this;
        }
    }
}
