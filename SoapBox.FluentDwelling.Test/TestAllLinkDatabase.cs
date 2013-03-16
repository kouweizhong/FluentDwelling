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
using NUnit.Framework;

namespace SoapBox.FluentDwelling.Test
{
    [TestFixture]
    public class TestAllLinkDatabase
    {
        const byte ALL_LINK_GROUP = 0x01;
        const byte ID1_HI = 0x12;
        const byte ID1_MIDDLE = 0x34;
        const byte ID1_LO = 0x56;
        const string ID1 = "12.34.56";
        const byte ID2_HI = 0x78;
        const byte ID2_MIDDLE = 0x9A;
        const byte ID2_LO = 0xBC;
        const string ID2 = "78.9A.BC";
        const byte FLAGS = Constants.ALL_LINK_DB_FLAGS_TYPICAL;
        const byte FLAGS_NOT_IN_USE = FLAGS & (255 - Constants.ALL_LINK_DB_FLAGS_RECORD_IN_USE);
        const byte FLAGS_NOT_MASTER = FLAGS & (255 - Constants.ALL_LINK_DB_FLAGS_PLM_IS_MASTER);

        [Test]
        public void Can_get_empty_database()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x69)
                    .AndReceive(0x02, 0x69, 0x15); // db empty

                var test = buildObjectForTest(scenario.Playback());
                Assert.AreEqual(0, test.Records.Count);
            }
        }

        [Test]
        public void Can_get_a_database_with_one_record()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x69)
                    .AndReceive(0x02, 0x69, 0x06); // has at least one record

                scenario
                    .WaitsForMessageOfType(0x57)
                    .AndReceives(0x02, 0x57, FLAGS_NOT_MASTER, ALL_LINK_GROUP, ID1_HI, ID1_MIDDLE, ID1_LO, 0xAA, 0xBB, 0xCC);

                scenario
                    .ShouldSend(0x02, 0x6A)
                    .AndReceive(0x02, 0x6A, 0x15); // no more

                var test = buildObjectForTest(scenario.Playback());
                Assert.AreEqual(1, test.Records.Count);

                var record = test.Records[0];
                Assert.IsFalse(record.PlmIsMaster);
                Assert.AreEqual(0xAA, record.LinkSpecificData(0));
                Assert.AreEqual(0xBB, record.LinkSpecificData(1));
                Assert.AreEqual(0xCC, record.LinkSpecificData(2));
            }
        }

        [Test]
        public void Can_get_a_database_with_two_records()
        {
            using (var scenario = new SerialPortScenario())
            {
                scenario
                    .ShouldSend(0x02, 0x69)
                    .AndReceive(0x02, 0x69, 0x06); // has at least one record

                scenario
                    .WaitsForMessageOfType(0x57)
                    .AndReceives(0x02, 0x57, FLAGS, ALL_LINK_GROUP, ID1_HI, ID1_MIDDLE, ID1_LO, 0, 0, 0);

                scenario
                    .ShouldSend(0x02, 0x6A)
                    .AndReceive(0x02, 0x6A, 0x06); // more

                scenario
                    .WaitsForMessageOfType(0x57)
                    .AndReceives(0x02, 0x57, FLAGS_NOT_IN_USE, ALL_LINK_GROUP, ID2_HI, ID2_MIDDLE, ID2_LO, 0, 0, 0);

                scenario
                    .ShouldSend(0x02, 0x6A)
                    .AndReceive(0x02, 0x6A, 0x15); // no more

                var test = buildObjectForTest(scenario.Playback());
                Assert.AreEqual(2, test.Records.Count);

                var record1 = test.Records[0];
                Assert.AreEqual(ID1, record1.DeviceId.ToString());
                Assert.AreEqual(ALL_LINK_GROUP, record1.AllLinkGroup);
                Assert.IsTrue(record1.InUse);
                Assert.IsTrue(record1.PlmIsMaster);

                var record2 = test.Records[1];
                Assert.AreEqual(ID2, record2.DeviceId.ToString());
                Assert.AreEqual(ALL_LINK_GROUP, record2.AllLinkGroup);
                Assert.IsFalse(record2.InUse);
                Assert.IsTrue(record2.PlmIsMaster);

            }
        }

        private PlmAllLinkDatabase buildObjectForTest(ISerialPortController serialPortController)
        {
            return new Plm(serialPortController).GetAllLinkDatabase();
        }
    }
}
