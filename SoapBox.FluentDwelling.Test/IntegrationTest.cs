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
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using System.Threading;
using System.Diagnostics;
using SoapBox.FluentDwelling.Devices;

namespace SoapBox.FluentDwelling.Test
{
    /// <summary>
    /// These integration tests are meant to be run with a PLM
    /// attached to your USB port, running the FTDI Chip VCP
    /// drivers.  Set COMPORT_NAME to your virtual com port.
    /// </summary>
    [TestFixture]
    public class IntegrationTest
    {
        private string _serialPort;
        private static readonly DeviceId peerId = new DeviceId(0x13, 0x55, 0x05); // some other device on the network

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var serialPort in theSerialPortNames)
            {
                using (var plm = new Plm(serialPort))
                {
                    plm.GetInfo();
                    if (!plm.Error)
                    {
                        _serialPort = serialPort;
                    }
                }
            }

            if (_serialPort == null)
            {
              throw new Exception("No PLM was found.  Please make sure your PLM is plugged in to power and your serial/USB port.");
            }
        }

        [Test]
        public void Integration_test_GetInfo()
        {
            using (var plm = new Plm(_serialPort))
            {
                var info = plm.GetInfo();
                Assert.IsFalse(plm.Error);
                Assert.AreNotEqual(0, info.DeviceCategoryCode);
                Assert.AreNotEqual(0, info.DeviceSubcategoryCode);
                Assert.AreNotEqual(0, info.FirmwareVersion);
                Assert.AreEqual("Network Bridges", info.DeviceCategory);
                const string pattern = "PowerLinc - (Serial|USB) \\(Dual Band\\) \\[2413[SU]\\]";
                Assert.IsTrue(new Regex(pattern).IsMatch(info.DeviceSubcategory));
            }
        }

        [Test]
        public void Integration_test_LED()
        {
            using (var plm = new Plm(_serialPort))
            {
                plm.Led
                    .EnableManualControl()
                    .TurnOn();
                Assert.IsTrue(plm.GetConfiguration().ManualLedControl);

                Thread.Sleep(250);

                plm.Led
                    .TurnOff()
                    .DisableManualControl();
                Assert.IsFalse(plm.GetConfiguration().ManualLedControl);
                Assert.IsFalse(plm.Error);
            }
        }

        [Test]
        public void Integration_test_AllLinkDatabase()
        {
            using (var plm = new Plm(_serialPort))
            {
                var database = plm.GetAllLinkDatabase();
                Assert.IsFalse(plm.Error);
                Assert.IsNotNull(database);

                foreach (var record in database.Records)
                {
                  Debug.Print("Device Id: {0}", record.DeviceId);
                  DeviceBase device;
                  if (plm.Network.TryConnectToDevice(record.DeviceId, out device))
                  {
                    Debug.Print("\tAll Link Database");
                    foreach (var subRecord in device.GetAllLinkDatabase().Records)
                    {
                      Debug.Print("\t\tDevice ID: {0}", subRecord.DeviceId);
                    }
                    if (!device.AllLinkDatabase.Records.Any())
                    {
                      Debug.Print("\t\tNot paired with any devices!");
                    }
                    Debug.Print("\tPeer Device Category: {0}", device.DeviceCategory);
                    Debug.Print("\tPeer Device Subcategory: {0}", device.DeviceSubcategory);
                  }
                  else
                  {
                     Debug.Print("\tFailed to connect to device!");
                  }
                }
            }
        }

        [Test]
        public void Integration_test_Network()
        {
            // This test requires a device at peerId attached to your Insteon Network
            // If you don't have one, just disable or disregard this test.
            using (var plm = new Plm(_serialPort))
            {
                DeviceBase device;
                Assert.IsTrue(plm.Network
                    .TryConnectToDevice(peerId, out device));
                Assert.IsNotNull(device);
                Assert.AreEqual(peerId, device.DeviceId);
                Debug.Print("Network integration test - Peer Device info:");
                Debug.Print("Device Id: {0}", device.DeviceId.ToString());
                Debug.Print("Peer Device Category: {0}", device.DeviceCategory);
                Debug.Print("Peer Device Subcategory: {0}", device.DeviceSubcategory);

                // test Ping
                Assert.IsTrue(device.Ping());

                // Device category-specific integration testing
                if (device is LightingControl)
                {
                    Integration_test_LightingDevice(device as LightingControl);
                }
            }
        }

        public void Integration_test_LightingDevice(LightingControl device)
        {
            device.TurnOn();
            Thread.Sleep(150);
            device.TurnOff();
        }
    }
}
