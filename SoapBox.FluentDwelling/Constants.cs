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
    internal static class Constants
    {
        public const byte CONFIG_DISABLE_AUTO_LINKING = 0x80;
        public const byte CONFIG_MONITOR_MODE = 0x40;
        public const byte CONFIG_MANUAL_LED_CONTROL = 0x20;
        public const byte CONFIG_DISABLE_RS232_DEADMAN = 0x10;
        public const byte ALL_LINK_DB_FLAGS_RECORD_IN_USE = 0x80;
        public const byte ALL_LINK_DB_FLAGS_PLM_IS_MASTER = 0x40;
        public const byte ALL_LINK_DB_FLAGS_ACK_REQUIRED = 0x20;
        public const byte ALL_LINK_DB_FLAGS_RECORD_USED_EVER = 0x02;
        public const byte ALL_LINK_DB_FLAGS_TYPICAL =
            ALL_LINK_DB_FLAGS_RECORD_IN_USE |
            ALL_LINK_DB_FLAGS_PLM_IS_MASTER |
            ALL_LINK_DB_FLAGS_ACK_REQUIRED |
            ALL_LINK_DB_FLAGS_RECORD_USED_EVER;
        public const byte MSG_FLAGS_BROADCAST = 0x80;
        public const byte MSG_FLAGS_DIRECT = 0x00;
        public const byte MSG_FLAGS_DIRECT_ACK = 0x20;
        public const byte MSG_FLAGS_DIRECT_NACK = 0xA0;
        public const byte MSG_FLAGS_GROUP = 0x40;
        public const byte MSG_FLAGS_GROUP_BROADCAST = MSG_FLAGS_BROADCAST | MSG_FLAGS_GROUP;
        public const byte MSG_FLAGS_GROUP_CLEANUP = 0x40;
        public const byte MSG_FLAGS_GROUP_CLEANUP_ACK = 0x60;
        public const byte MSG_FLAGS_GROUP_CLEANUP_NACK = 0xE0;
        public const byte MSG_FLAGS_EXTENDED = 0x10;
        public const byte MSG_FLAGS_MAX_HOPS = 0x0F;

        public const byte ACK = 0x06;
        public const byte NACK = 0x15;

        public static readonly Dictionary<byte, string> DeviceCategoryLookup
            = new Dictionary<byte, string>()
            {
                { 0x00, "Generalized Controllers" },
                { 0x01, "Dimmable Lighting Control" },
                { 0x02, "Switched Lighting Control" },
                { 0x03, "Network Bridges" },
                { 0x04, "Irrigation Control" },
                { 0x05, "Climate Control" },
                { 0x06, "Pool and Spa Control" },
                { 0x07, "Sensors and Actuators" },
                { 0x08, "Home Entertainment" },
                { 0x09, "Energy Management" },
                { 0x0A, "Built-In Appliance Control" },
                { 0x0B, "Plumbing" },
                { 0x0C, "Communication" },
                { 0x0D, "Computer Control" },
                { 0x0E, "Window Coverings" },
                { 0x0F, "Access Control" },
                { 0x10, "Security, Health, Safety" },
                { 0x11, "Surveillance" },
                { 0x12, "Automotive" },
                { 0x13, "Pet Care" },
                { 0x14, "Toys" },
                { 0x15, "Timekeeping" },
                { 0x16, "Holiday" },
                { 0xFF, "Unassigned" }
            };

        public static readonly Dictionary<byte, Dictionary<byte, string>> DeviceSubcategoryLookup
            = new Dictionary<byte, Dictionary<byte, string>>()
            {
                { 0x00, new Dictionary<byte,string>() 
                    { 
                        {0x00, "Unknown"},
                        {0x04, "ControlLinc [2430]"},
                        {0x05, "RemoteLinc [2440]"},
                        {0x06, "Icon Tabletop Controller [2830]"},
                        {0x08, "EZBridge/EZServer"},
                        {0x09, "SignalLinc RF Signal Enhancer [2442]"},
                        {0x0A, "Balboa Instrument's Poolux LCD Controller"},
                        {0x0B, "Access Point [2443]"},
                        {0x0C, "IES Color Touchscreen"},
                        {0x0D, "SmartLabs KeyFOB"}
                    } },
                { 0x01, new Dictionary<byte,string>() 
                    { 
                        {0x00, "LampLinc V2 [2456D3]"},
                        {0x01, "SwitchLinc V2 Dimmer 600W [2476D]"},
                        {0x02, "In-LineLinc Dimmer [2475D]"},
                        {0x03, "Icon Switch Dimmer [2876D]"},
                        {0x04, "SwitchLinc V2 Dimmer 1000W [2476DH]"},
                        {0x05, "KeypadLinc Dimmer Countdown Timer [2484DWH8]"},
                        {0x06, "LampLinc 2-Pin [2456D2]"},
                        {0x07, "Icon LampLinc V2 2-Pin [2856D2]"},
                        {0x08, "SwitchLinc Dimmer Count-down Timer [2484DWH8]"},
                        {0x09, "KeypadLinc Dimmer [2486D]"},
                        {0x0A, "Icon In-Wall Controller [2886D]"},
                        {0x0B, "Access Point LampLinc [2458D3]"},
                        {0x0C, "KeypadLinc Dimmer - 8-Button defaulted mode [2486DWH8]"},
                        {0x0D, "SocketLinc [2454D]"},
                        {0x0E, "LampLinc Dimmer, Dual-Band [2457D3]"},
                        {0x13, "ICON SwitchLinc Dimmer for Lixar/Bell Canada [2676D-B]"},
                        {0x17, "ToggleLinc Dimmer [2466D]"},
                        {0x18, "Icon SL Dimmer Inline Companion [2474D]"},
                        {0x19, "SwitchLinc 800W"},
                        {0x1A, "In-LineLinc Dimmer with Sense [2475D2]"},
                        {0x1B, "KeypadLinc 6-button Dimmer [2486DWH6]"},
                        {0x1C, "KeypadLinc 8-button Dimmer [2486DWH8]"},
                        {0x1D, "SwitchLinc Dimmer 1200W [2476D]"}
                    } },
                { 0x02, new Dictionary<byte,string>() 
                    { 
                        {0x05, "KeypadLinc Relay - 8-Button defaulted mode [2486SWH8]"},
                        {0x06, "Outdoor ApplianceLinc [2456S3E]"},
                        {0x07, "TimerLinc [2456ST3]"},
                        {0x08, "OutletLinc [2473S]"},
                        {0x09, "ApplianceLinc [2456S3]"},
                        {0x0A, "SwitchLinc Relay [2476S]"},
                        {0x0B, "Icon On Off Switch [2876S]"},
                        {0x0C, "Icon Appliance Adapter [2856S3]"},
                        {0x0D, "ToggleLinc Relay [2466S]"},
                        {0x0E, "SwitchLinc Relay Countdown Timer [2476ST]"},
                        {0x0F, "KeypadLinc On/Off Switch [2486SWH6]"},
                        {0x10, "In-LineLinc Relay [2475D]"},
                        {0x11, "EZSwitch30 (240V, 30A load controller)"},
                        {0x12, "Icon SL Relay Inline Companion"},
                        {0x13, "ICON SwitchLinc Relay for Lixar/Bell Canada [2676R-B]"},
                        {0x14, "In-LineLinc Relay with Sense [2475S2]"},
                        {0x16, "SwitchLinc Relay with Sense [2476S2]"}
                    } },
                { 0x03, new Dictionary<byte,string>() 
                    { 
                        {0x01, "PowerLinc Serial [2414S]"},
                        {0x02, "PowerLinc USB [2414U]"},
                        {0x03, "Icon PowerLinc Serial [2814S]"},
                        {0x04, "Icon PowerLinx USB [2814U]"},
                        {0x05, "SmartLabs PowerLinc Modem Serial [2412S]"},
                        {0x06, "SmartLabs IR to Insteon Interface [2411R]"},
                        {0x07, "SmartLabs IRLinc - IR Transmitter Interface [2411T]"},
                        {0x08, "SmartLabs Bi-Directional IR -Insteon Interface"},
                        {0x09, "SmartLabs RF Developer's Board [2600RF]"},
                        {0x0A, "SmartLabs PowerLinc Modem Ethernet [2412E]"},
                        {0x0B, "SmartLabs PowerLinc Modem USB [2412U]"},
                        {0x0C, "SmartLabs PLM Alert Serial"},
                        {0x0D, "SimpleHomeNet EZX 10RF"},
                        {0x0E, "X10 TW-523/PSC05 Translator"},
                        {0x0F, "EZX10IR (X10 IR receiver, Insteon controller and IR distribution hub)"},
                        {0x10, "SmartLinc 2412N INSTEON Central Controller"},
                        {0x11, "PowerLinc - Serial (Dual Band) [2413S]"},
                        {0x12, "RF Modem Card"},
                        {0x13, "PowerLinc USB - HouseLinc 2 enabled [2412UH]"},
                        {0x14, "PowerLinc Serial - HouseLinc 2 enabled [2412SH]"},
                        {0x15, "PowerLinc - USB (Dual Band) [2413U]"}
                    } },
                { 0x04, new Dictionary<byte,string>() 
                    { 
                        {0x00, "Compacta EZRain Sprinkler Controller"}
                    } },
                { 0x05, new Dictionary<byte,string>() 
                    { 
                        {0x00, "Broan SMSC080 Exhaust Fan"},
                        {0x01, "Compacta EZTherm"},
                        {0x02, "Broan SMSC110 Exhaust Fan"},
                        {0x03, "INSTEON Thermostat Adapter [2441V]"},
                        {0x04, "Compacta EZThermx Thermostat"},
                        {0x05, "Broan, Venmar, BEST Rangehoods"},
                        {0x06, "Broan SmartSense Make-up Damper"}
                    } },
                { 0x06, new Dictionary<byte,string>() 
                    { 
                        {0x00, "Compacta EZPool"},
                        {0x01, "Low-end pool controller (Temp. Eng. Project name)"},
                        {0x02, "Mid-Range pool controller (Temp. Eng. Project name)"},
                        {0x03, "Next Generation pool controller (Temp. Eng. Project name)"}
                    } },
                { 0x07, new Dictionary<byte,string>() 
                    { 
                        {0x00, "IOLinc [2450]"},
                        {0x01, "Compacta EZSns1W Sensor Interface Module"},
                        {0x02, "Compacta EZIO8T I/O Module"},
                        {0x03, "Compacta EZIO2X4 #5010D INSTEON / X10 Input/Output Module"},
                        {0x04, "Compacta EZIO8SA I/O Module"},
                        {0x05, "Compacta EZSnsRF #5010E RF Receiver Interface Module for Dakota Alerts Products"},
                        {0x06, "Compacta EZISnsRf Sensor Interface Module"},
                        {0x07, "EZIO6I (6 inputs)"},
                        {0x08, "EZIO4O (4 relay outputs)"}
                    } },
                { 0x09, new Dictionary<byte,string>() 
                    { 
                        {0x00, "Compacta EZEnergy"},
                        {0x01, "OnSitePro Leak Detector"},
                        {0x02, "OnsitePro Control Valve"},
                        {0x03, "Energy Inc. TED 5000 Single Phase Measuring Transmitting Unit (MTU)"},
                        {0x04, "Energy Inc. TED 5000 Gateway - USB"},
                        {0x05, "Energy Inc. TED 5000 Gateway - Ethernet"},
                        {0x06, "Energy Inc. TED 3000 Three Phase Measuring Transmitting Unit (MTU)"},
                    } },
                { 0x0E, new Dictionary<byte,string>() 
                    { 
                        {0x00, "Somfy Drape Controller RF Bridge"}
                    } },
                { 0x0F, new Dictionary<byte,string>() 
                    { 
                        {0x00, "Weiland Doors' Central Drive and Controller"},
                        {0x01, "Weiland Doors' Secondary Central Drive"},
                        {0x02, "Weiland Doors' Assist Drive"},
                        {0x03, "Weiland Doors' Elevation Drive"}
                    } },
                { 0x10, new Dictionary<byte,string>() 
                    { 
                        {0x00, "First Alert ONELink RF to Insteon Bridge"},
                        {0x01, "Motion Sensor [2420M]"},
                        {0x02, "TriggerLinc - INSTEON Open / Close Sensor [2421]"}
                    } }
            };

        public static readonly Dictionary<byte, int> IncomingMessageLengthLookup
            = new Dictionary<byte, int>()
            {
                { 0x50, 11 },
                { 0x51, 25 },
                { 0x52, 4 },
                { 0x53, 10 },
                { 0x54, 3 },
                { 0x55, 2 },
                { 0x56, 13 },
                { 0x57, 10 },
                { 0x58, 3 },
                { 0x60, 9 },
                { 0x61, 6 },
                { 0x62, 23 }, // could get 9 or 23 (Standard or Extended Message received)
                { 0x63, 5 },
                { 0x64, 5 },
                { 0x65, 3 },
                { 0x66, 6 },
                { 0x67, 3 },
                { 0x68, 4 },
                { 0x69, 3 },
                { 0x6A, 3 },
                { 0x6B, 4 },
                { 0x6C, 3 },
                { 0x6D, 3 },
                { 0x6E, 3 },
                { 0x6F, 12 },
                { 0x70, 4 },
                { 0x71, 5 },
                { 0x72, 3 },
                { 0x73, 6 }
            };

        public static readonly Dictionary<string, byte> X10HouseCodeLookup
            = new Dictionary<string, byte>()
            {
                { "A", 0x60 },
                { "B", 0xE0 },
                { "C", 0x20 },
                { "D", 0xA0 },
                { "E", 0x10 },
                { "F", 0x90 },
                { "G", 0x50 },
                { "H", 0xD0 },
                { "I", 0x70 },
                { "J", 0xF0 },
                { "K", 0x30 },
                { "L", 0xB0 },
                { "M", 0x00 },
                { "N", 0x80 },
                { "O", 0x40 },
                { "P", 0xC0 }
            };

        public static readonly Dictionary<byte, string> X10HouseCodeReverseLookup
            = new Dictionary<byte, string>()
            {
                { 0x60, "A" },
                { 0xE0, "B" },
                { 0x20, "C" },
                { 0xA0, "D" },
                { 0x10, "E" },
                { 0x90, "F" },
                { 0x50, "G" },
                { 0xD0, "H" },
                { 0x70, "I" },
                { 0xF0, "J" },
                { 0x30, "K" },
                { 0xB0, "L" },
                { 0x00, "M" },
                { 0x80, "N" },
                { 0x40, "O" },
                { 0xC0, "P" }
            };

        public static readonly Dictionary<byte, byte> X10UnitCodeLookup
            = new Dictionary<byte, byte>()
            {
                { 1, 0x06 },
                { 2, 0x0E },
                { 3, 0x02 },
                { 4, 0x0A },
                { 5, 0x01 },
                { 6, 0x09 },
                { 7, 0x05 },
                { 8, 0x0D },
                { 9, 0x07 },
                { 10, 0x0F },
                { 11, 0x03 },
                { 12, 0x0B },
                { 13, 0x00 },
                { 14, 0x08 },
                { 15, 0x04 },
                { 16, 0x0C },
            };

        public static readonly Dictionary<byte, byte> X10UnitCodeReverseLookup
            = new Dictionary<byte, byte>()
            {
                { 0x06, 1 },
                { 0x0E, 2 },
                { 0x02, 3 },
                { 0x0A, 4 },
                { 0x01, 5 },
                { 0x09, 6 },
                { 0x05, 7 },
                { 0x0D, 8 },
                { 0x07, 9 },
                { 0x0F, 10 },
                { 0x03, 11 },
                { 0x0B, 12 },
                { 0x00, 13 },
                { 0x08, 14 },
                { 0x04, 15 },
                { 0x0C, 16 },
            };
    }
}
