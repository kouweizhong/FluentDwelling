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

namespace SoapBox.FluentDwelling.Devices
{
    public class IrrigationControl : DeviceBase
    {
        internal IrrigationControl(Plm plm, DeviceId deviceId,
            byte deviceCategory, byte deviceSubcategory)
            : base(plm, deviceId, deviceCategory, deviceSubcategory)
        {
        }

        /// <summary>
        /// Commands the irrigation device to turn on the given
        /// sprinkler valve.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOnSprinklerValve(byte valveNumber)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x40, valveNumber);
        }

        /// <summary>
        /// Commands the irrigation device to turn off the given
        /// sprinkler valve.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOffSprinklerValve(byte valveNumber)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x41, valveNumber);
        }

        /// <summary>
        /// Commands the irrigation device to turn on the given
        /// sprinkler program.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOnSprinklerProgram(byte programNumber)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x42, programNumber);
        }

        /// <summary>
        /// Commands the irrigation device to turn off the given
        /// sprinkler program.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TurnOffSprinklerProgram(byte programNumber)
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x43, programNumber);
        }

        /// <summary>
        /// Commands the irrigation device to load initialization values.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool LoadInitializationValues()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x00);
        }

        /// <summary>
        /// Commands the irrigation device to Load EEPROM parameters from RAM.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool LoadEepromFromRam()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x01);
        }

        /// <summary>
        /// Commands the irrigation device to read the status of the valves.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool TryGetValveStatus(out byte valveStatus)
        {
            bool result = false;
            byte retrievedValveStatus = 0;
            base.Plm.exceptionHandler(() =>
            {
                byte[] responseAck = base.Plm.sendStandardLengthMessageAndWait4Response(
                        base.DeviceId, Constants.MSG_FLAGS_DIRECT, 0x44, 0x02);
                byte flags = DeviceMessage.MessageFlags(responseAck);
                result = (flags & Constants.MSG_FLAGS_DIRECT_ACK) > 0;
                retrievedValveStatus = DeviceMessage.Command2(responseAck);
            });
            valveStatus = retrievedValveStatus;
            return result;
        }

        /// <summary>
        /// Commands the irrigation device to stop accepting commands.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool InhibitCommandAcceptance()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x03);
        }

        /// <summary>
        /// Commands the irrigation device to resume accepting commands.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool ResumeCommandAcceptance()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x04);
        }

        /// <summary>
        /// Commands the irrigation device to turn off the active valve and 
        /// continue with the next valve in program.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool SkipForward()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x05);
        }

        /// <summary>
        /// Commands the irrigation device to turn off the active valve and 
        /// continue with the previous valve in program.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool SkipBack()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x06);
        }

        /// <summary>
        /// Commands the irrigation device to enable pump on valve(?) 8.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool EnablePumpOnV8()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x07);
        }

        /// <summary>
        /// Commands the irrigation device to disable pump on valve(?) 8.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool DisablePumpOnV8()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x08);
        }

        /// <summary>
        /// Commands the irrigation device to enable a broadcast message
        /// of "status changed" whenever a valve status changes.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool EnableDeviceStatusChangedBroadcast()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x09);
        }

        /// <summary>
        /// Commands the irrigation device to disable the broadcast message
        /// of "status changed" whenever a valve status changes.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool DisableDeviceStatusChangedBroadcast()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x0A);
        }

        /// <summary>
        /// Commands the irrigation device to Load RAM parameters from EEPROM.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool LoadRamFromEeprom()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x0B);
        }

        /// <summary>
        /// Commands the irrigation device to enable the sensor reading.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool EnableSensorReading()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x0C);
        }

        /// <summary>
        /// Commands the irrigation device to disable the sensor reading.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool DisableSensorReading()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x0D);
        }

        /// <summary>
        /// Commands the irrigation device to enter self-diagnostics mode.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool DiagnosticsOn()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x0E);
        }

        /// <summary>
        /// Commands the irrigation device to exit from self-diagnostics mode.
        /// </summary>
        /// <returns>True if the device responds with an ACK</returns>
        public bool DiagnosticsOff()
        {
            return base.Plm.Network
                .SendStandardCommandToAddress(base.DeviceId, 0x44, 0x0F);
        }


    }
}
