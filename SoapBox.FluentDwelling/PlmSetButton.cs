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
    public class PlmSetButton
    {
        private readonly Plm plm;

        internal PlmSetButton(Plm plm)
        {
            if (plm == null) throw new ArgumentNullException("plm");
            this.plm = plm;
        }

        /// <summary>
        /// This event is fired if someone "taps" the SET button
        /// on the PLM.  A tap is when they press the button
        /// but release it before entering all-linking mode.
        /// </summary>
        public event EventHandler Tapped;

        private void fireTappedEvent()
        {
            var evt = Tapped;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        internal void setButtonTapped()
        {
            fireTappedEvent();
        }

        /// <summary>
        /// This event is fired if someone presses and
        /// holds the SET button on the PLM long enough
        /// that they enter (or would enter) all-linking mode.
        /// </summary>
        public event EventHandler PressedAndHeld;

        private void firePressedAndHeldEvent()
        {
            var evt = PressedAndHeld;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        internal void setButtonPressedAndHeld()
        {
            firePressedAndHeldEvent();
        }

        /// <summary>
        /// This event is fired after the PressedAndHeld
        /// event, when the person holding the button
        /// releases it.
        /// </summary>
        public event EventHandler ReleasedAfterHolding;

        private void fireReleasedAfterHoldingEvent()
        {
            var evt = ReleasedAfterHolding;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        internal void setButtonReleasedAfterHolding()
        {
            fireReleasedAfterHoldingEvent();
        }

        /// <summary>
        /// This event is fired after the PressedAndHeld
        /// event, when the person holding the button
        /// releases it.
        /// </summary>
        public event EventHandler UserReset;

        private void fireUserResetEvent()
        {
            var evt = UserReset;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        internal void setButtonHeldDuringPowerUp()
        {
            fireUserResetEvent();
        }
    }
}
