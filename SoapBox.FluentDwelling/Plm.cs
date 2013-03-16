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
    public class Plm : PlmCommunicatorBase
    {
        private readonly PlmLed led;
        private readonly PlmSetButton setButton;
        private readonly PlmNetwork network;

        /// <summary>
        /// Creates a new object for reading from and writing
        /// to an Powerline Modem (PLM) over a
        /// serial port (or a virtual com port using the 
        /// FTDI chip VCP drivers).
        /// </summary>
        public Plm()
          : this(new SerialPortController(DiscoverComPort()))
        { }

        internal static string DiscoverComPort()
        {
          string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();
          foreach (var serialPort in theSerialPortNames)
          {
            using (var plm = new Plm(serialPort))
            {
              plm.GetInfo();
              if (!plm.Error)
              {
                return serialPort;
              }
            }
          }

          throw new Exception("No PLM was found.  Please make sure your PLM is plugged in to power and your serial/USB port.");
        }

        /// <summary>
        /// Creates a new object for reading from and writing
        /// to an Powerline Modem (PLM) over a
        /// serial port (or a virtual com port using the 
        /// FTDI chip VCP drivers).
        /// </summary>
        /// <param name="comPortName">Example: "COM4"</param>
        public Plm(string comPortName)
            : this(new SerialPortController(comPortName))
        { }

        internal Plm(ISerialPortController serialPortController)
            : base(serialPortController)
        {
            this.led = new PlmLed(this);
            this.setButton = new PlmSetButton(this);
            this.network = new PlmNetwork(this);
        }

        /// <summary>
        /// Access the control features for the LED on 
        /// the PLM itself.
        /// </summary>
        public PlmLed Led { get { return this.led; } }

        /// <summary>
        /// Access the control features for the SET button on 
        /// the PLM itself.
        /// </summary>
        public PlmSetButton SetButton { get { return this.setButton; } }

        /// <summary>
        /// Access the RF and power-line network interface
        /// </summary>
        public PlmNetwork Network { get { return this.network; } }
        
        /// <summary>
        /// Reads the PLM's configuration byte and 
        /// then decodes it and returns it as 
        /// a PlmConfiguration object.
        /// </summary>
        public PlmConfiguration GetConfiguration()
        {
            byte configuration = 0;
            exceptionHandler(() =>
                {
                    configuration = getIMConfiguration();
                });
            return new PlmConfiguration(configuration);
        }

        /// <summary>
        /// Reads the PLM's information including its
        /// Device ID, category, and firmware rev, and
        /// returns it in a PlmInfo object.
        /// </summary>
        public PlmInfo GetInfo()
        {
            var info = new PlmInfo(this); // creates value with all zeros
            exceptionHandler(() =>
            {
                info = getIMInfo(this);
            });
            return info;
        }

        /// <summary>
        /// Reads the list of linked devices from the PLM's 
        /// memory and returns it as a list of records.
        /// </summary>
        public PlmAllLinkDatabase GetAllLinkDatabase()
        {
            var database = new PlmAllLinkDatabase(); // creates empty database
            exceptionHandler(() =>
            {
                database = getAllLinkDatabase();
            });
            return database;
        }

        /// <summary>
        /// In order for incoming events to work (button presses,
        /// incoming messages, etc.), the client code has to call
        /// this at regular intervals.  The events will fire on
        /// the same thread that calls this method.
        /// </summary>
        public void Receive()
        {
            exceptionHandler(() =>
            {
                receiveAndQueueIncomingMessage();
                processReceivedMessageQueue();
            });
        }

        protected override void processIncomingMessage(byte[] message)
        {
            DeviceId peerId;
            byte group;
            if (message.Length < 2) return;

            switch (message[1])
            {
                case 0x50:
                    if (message.Length != 11) return;
                    peerId = new DeviceId(message[2], message[3], message[4]);
                    group = message[7];
                    byte flags = message[8];
                    byte command1 = message[9];
                    byte command2 = message[10];
                    this.network.standardMessageReceived(peerId, group, flags, command1, command2);
                    break;
                case 0x52:
                    if (message.Length != 4) return;
                    this.network.X10.x10MessageReceived(message[2], message[3]);
                    break;
                case 0x53:
                    if (message.Length != 10) return;
                    byte linkCode = message[2];
                    group = message[3];
                    peerId = new DeviceId(message[4], message[5], message[6]);
                    byte deviceCategory = message[7];
                    byte deviceSubcategory = message[8];
                    this.network.allLinkingCompleted(linkCode, group, peerId, deviceCategory, deviceSubcategory);
                    break;
                case 0x54:
                    if (message.Length != 3) return;
                    if (message[2] == 0x02) // set button pressed
                    {
                        this.setButton.setButtonTapped();
                    }
                    else if (message[2] == 0x03) // set button pressed and held
                    {
                        this.setButton.setButtonPressedAndHeld();
                    }
                    else if (message[2] == 0x04) // set button released after being held
                    {
                        this.setButton.setButtonReleasedAfterHolding();
                    }
                    break;
                case 0x55:
                    this.setButton.setButtonHeldDuringPowerUp();
                    break;
            }
        }

    }
}
