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
    public class TestStandardMessageReceivedArgs
    {

        [Test]
        public void Decodes_flags()
        {
            Assert.IsFalse(buildWithFlags(0x00).Flags.Broadcast);
            Assert.IsFalse(buildWithFlags(0x00).Flags.Group);
            Assert.IsFalse(buildWithFlags(0x00).Flags.Acknowledge);
            Assert.IsFalse(buildWithFlags(0x00).Flags.Extended);
            Assert.AreEqual(0, buildWithFlags(0x00).Flags.HopsLeft);
            Assert.AreEqual(0, buildWithFlags(0x00).Flags.MaxHops);

            Assert.IsTrue(buildWithFlags(Constants.MSG_FLAGS_BROADCAST).Flags.Broadcast);
            Assert.IsTrue(buildWithFlags(Constants.MSG_FLAGS_GROUP).Flags.Group);
            Assert.IsTrue(buildWithFlags(Constants.MSG_FLAGS_DIRECT_ACK).Flags.Acknowledge);
            Assert.IsTrue(buildWithFlags(Constants.MSG_FLAGS_EXTENDED).Flags.Extended);
            Assert.AreEqual(3, buildWithFlags(0x0C).Flags.HopsLeft);
            Assert.AreEqual(3, buildWithFlags(0x03).Flags.MaxHops);
        }

        [Test]
        public void Decodes_message_type()
        {
            Assert.AreEqual(StandardMessageType.Broadcast, buildWithFlags(Constants.MSG_FLAGS_BROADCAST).MessageType);
            Assert.AreEqual(StandardMessageType.Direct, buildWithFlags(Constants.MSG_FLAGS_DIRECT).MessageType);
            Assert.AreEqual(StandardMessageType.AckOfDirect, buildWithFlags(Constants.MSG_FLAGS_DIRECT_ACK).MessageType);
            Assert.AreEqual(StandardMessageType.NackOfDirect, buildWithFlags(Constants.MSG_FLAGS_DIRECT_NACK).MessageType);
            Assert.AreEqual(StandardMessageType.GroupBroadcast, buildWithFlags(Constants.MSG_FLAGS_GROUP_BROADCAST).MessageType);
            Assert.AreEqual(StandardMessageType.GroupCleanupDirect, buildWithFlags(Constants.MSG_FLAGS_GROUP_CLEANUP).MessageType);
            Assert.AreEqual(StandardMessageType.AckOfGroupCleanupDirect, buildWithFlags(Constants.MSG_FLAGS_GROUP_CLEANUP_ACK).MessageType);
            Assert.AreEqual(StandardMessageType.NackOfGroupCleanupDirect, buildWithFlags(Constants.MSG_FLAGS_GROUP_CLEANUP_NACK).MessageType);
        }

        [Test]
        public void Decodes_group_number_from_certain_message_types()
        {
            Assert.AreEqual(4, buildObjectForTest(4, Constants.MSG_FLAGS_GROUP_BROADCAST, 0x11, 0x80).Group);
            Assert.AreEqual(0, buildObjectForTest(4, Constants.MSG_FLAGS_BROADCAST, 0x11, 0x80).Group);
            Assert.AreEqual(3, buildObjectForTest(4, Constants.MSG_FLAGS_GROUP_CLEANUP, 0x11, 0x03).Group);
        }

        private StandardMessageReceivedArgs buildWithFlags(byte flags)
        {
            var deviceId = new DeviceId(0x10, 0x20, 0x30);
            return new StandardMessageReceivedArgs(deviceId, 0, flags, 0x11, 0x00);
        }

        private StandardMessageReceivedArgs buildObjectForTest(byte group, byte flags, byte command1, byte command2)
        {
            var deviceId = new DeviceId(0x10, 0x20, 0x30);
            return new StandardMessageReceivedArgs(deviceId, group, flags, command1, command2);
        }
    }
}
