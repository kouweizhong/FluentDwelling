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
    public class TestIncomingMessageBuffer
    {
        [Test]
        public void Buffers_bytes_to_form_a_complete_message()
        {
            byte[] testMessage = new byte[] { 0x02, 0x67, 0x06 }; // Reset IM ack

            var test = buildObjectForTest();

            test.AddBytes(testMessage[0]);

            IncomingMessage result;
            Assert.IsFalse(test.TryGetIncomingMessage(out result));
            Assert.IsNull(result);

            test.AddBytes(testMessage[1], testMessage[2]);

            Assert.IsTrue(test.TryGetIncomingMessage(out result));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x67, result.MessageType);
            Assert.AreEqual(testMessage, result.Message);
        }

        [Test]
        public void Throws_away_garbage_data_before_messages()
        {
            byte[] testMessage = new byte[] { 0x02, 0x67, 0x06 }; // Reset IM ack

            var test = buildObjectForTest();

            test.AddBytes(0x02);
            test.AddBytes(testMessage);

            IncomingMessage result;
            Assert.IsTrue(test.TryGetIncomingMessage(out result));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x67, result.MessageType);
            Assert.AreEqual(testMessage, result.Message);
        }

        [Test]
        public void Throws_away_garbage_data_between_messages()
        {
            byte[] testMessage1 = new byte[] { 0x02, 0x67, 0x06 }; // Reset IM ack
            byte[] testMessage2 = new byte[] { 0x02, 0x65, 0x06 }; // Cancel All-Linking

            var test = buildObjectForTest();

            test.AddBytes(testMessage1);
            test.AddBytes(0x82);
            test.AddBytes(0xAF);
            test.AddBytes(testMessage2);

            IncomingMessage result;
            Assert.IsTrue(test.TryGetIncomingMessage(out result));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x67, result.MessageType);
            Assert.AreEqual(testMessage1, result.Message);

            Assert.IsTrue(test.TryGetIncomingMessage(out result));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x65, result.MessageType);
            Assert.AreEqual(testMessage2, result.Message);
        }

        [Test]
        public void Can_clear()
        {
            byte[] testMessage1 = new byte[] { 0x02, 0x67, 0x06 }; // Reset IM ack
            byte[] testMessage2 = new byte[] { 0x02, 0x65, 0x06 }; // Cancel All-Linking

            var test = buildObjectForTest();

            test.AddBytes(testMessage1);
            test.AddBytes(testMessage2[0]);
            test.Clear();
            test.AddBytes(testMessage2[1], testMessage2[2]);

            IncomingMessage result;
            Assert.IsFalse(test.TryGetIncomingMessage(out result));
            Assert.IsNull(result);
        }

        [Test]
        public void Can_get_by_message_type()
        {
            byte[] testMessage1 = new byte[] { 0x02, 0x67, 0x06 }; // Reset IM ack
            byte[] testMessage2 = new byte[] { 0x02, 0x65, 0x06 }; // Cancel All-Linking

            var test = buildObjectForTest();

            test.AddBytes(testMessage1);
            test.AddBytes(testMessage2);

            IncomingMessage result;
            Assert.IsTrue(test.TryGetMessageByType(out result, 0x65));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x65, result.MessageType);
            Assert.AreEqual(testMessage2, result.Message);

            Assert.IsTrue(test.TryGetIncomingMessage(out result));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x67, result.MessageType);
            Assert.AreEqual(testMessage1, result.Message);
        }

        [Test]
        public void Works_on_message_type_0x62() // the length cannot be determined by the message type alone
        {
            byte[] testMessage1 = new byte[] { 0x02, 0x62, 0, 0, 0, 0, 0, 0, 0x06 }; // Send Standard Message echo
            byte[] testMessage2 = new byte[] { 0x02, 0x62, 0, 0, 0, 0x10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0x06 }; // Send Extended Message echo

            Assert.AreEqual(9, testMessage1.Length);
            Assert.AreEqual(23, testMessage2.Length);

            var test = buildObjectForTest();

            test.AddBytes(testMessage1);
            test.AddBytes(testMessage2);

            IncomingMessage result;
            Assert.IsTrue(test.TryGetIncomingMessage(out result));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x62, result.MessageType);
            Assert.AreEqual(testMessage1, result.Message);

            Assert.IsTrue(test.TryGetIncomingMessage(out result));
            Assert.IsNotNull(result);
            Assert.AreEqual(0x62, result.MessageType);
            Assert.AreEqual(testMessage2, result.Message);
        }


        private static IncomingMessageBuffer buildObjectForTest()
        {
            return new IncomingMessageBuffer();
        }
    }
}
