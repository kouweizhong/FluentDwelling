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
    public class PlmConfiguration
    {
        internal PlmConfiguration(byte configuration)
        {
            this.AutoLinkingDisabled = (configuration & Constants.CONFIG_DISABLE_AUTO_LINKING) > 0;
            this.MonitorMode = (configuration & Constants.CONFIG_MONITOR_MODE) > 0;
            this.ManualLedControl = (configuration & Constants.CONFIG_MANUAL_LED_CONTROL) > 0;
            this.Rs232Deadman = (configuration & Constants.CONFIG_DISABLE_RS232_DEADMAN) > 0;
        }

        public bool AutoLinkingDisabled { get; private set; }
        public bool MonitorMode { get; private set; }
        public bool ManualLedControl { get; private set; }
        public bool Rs232Deadman { get; private set; }
    }
}
