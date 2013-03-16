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
    public class TestDeviceId
    {

        [Test]
        public void Test_Equality()
        {
            var id_123456_1 = new DeviceId(0x12, 0x34, 0x56);
            var id_123456_2 = new DeviceId(0x12, 0x34, 0x56);
            var id_AABBCC = new DeviceId(0xAA, 0xBB, 0xCC);

            Assert.AreEqual(id_123456_1, id_123456_2);
            Assert.AreNotEqual(id_123456_1, id_AABBCC);
            Assert.AreNotEqual(null, id_AABBCC);
            Assert.AreNotEqual(3.14, id_AABBCC);
            Assert.AreNotEqual(id_123456_1.ToString(), id_AABBCC);
            Assert.IsTrue(id_123456_1 == id_123456_2);
            Assert.IsFalse(id_123456_1 == id_AABBCC);
            Assert.IsFalse(id_123456_1 != id_123456_2);
            Assert.IsTrue(id_123456_1 != id_AABBCC);
        }

        [Test]
        public void Can_create_from_id_string()
        {
            const string ID1 = "12.34.56";
            const string ID2 = "AA.BB.Cc";
            var id_123456 = new DeviceId(ID1);
            var id_AABBCC = new DeviceId(ID2);

            Assert.AreEqual(0x12, id_123456.IdHi);
            Assert.AreEqual(0x34, id_123456.IdMiddle);
            Assert.AreEqual(0x56, id_123456.IdLo);
            Assert.AreEqual(ID1.ToUpper(), id_123456.ToString());

            Assert.AreEqual(0xAA, id_AABBCC.IdHi);
            Assert.AreEqual(0xBB, id_AABBCC.IdMiddle);
            Assert.AreEqual(0xCC, id_AABBCC.IdLo);
            Assert.AreEqual(ID2.ToUpper(), id_AABBCC.ToString());

            Assert.Throws<ArgumentNullException>(() => { var id = new DeviceId((string)null); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var id = new DeviceId("12;34.56"); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var id = new DeviceId("12.34;56"); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var id = new DeviceId("12.34.5"); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { var id = new DeviceId("12.34.5G"); });
        }

    }
}
