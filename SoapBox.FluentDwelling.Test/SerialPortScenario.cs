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
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace SoapBox.FluentDwelling.Test
{
    class SerialPortScenario : IDisposable
    {
        private readonly MockRepository mocks = new MockRepository();
        private readonly ISerialPortController port;

        public SerialPortScenario()
        {
            this.port = mocks.StrictMock<ISerialPortController>();
        }

        public SendContext ShouldSend(params byte[] send)
        {
            return new SendContext(this, send);
        }

        public SerialPortScenario IncomingMessage(params byte[] receive)
        {
            byte[] incomingMessage;
            Expect.Call(this.port.TryGetIncomingMessages(out incomingMessage))
                .IgnoreArguments().OutRef(receive)
                .Return(true);
            return this;
        }

        public WaitContext WaitsForMessageOfType(params byte[] messageTypes)
        {
            return new WaitContext(this, messageTypes);
        }

        public ISerialPortController Playback()
        {
            this.mocks.ReplayAll();
            return this.port;
        }

        public void Dispose()
        {
            this.mocks.VerifyAll();
        }

        public class SendContext
        {
            private readonly SerialPortScenario scenario;
            private readonly byte[] send;

            public SendContext(SerialPortScenario scenario, byte[] send)
            {
                this.scenario = scenario;
                this.send = send;
            }

            public SerialPortScenario AndReceive(params byte[] receive)
            {
                Expect.Call(this.scenario.port.SendReceive(this.send, receive.Length))
                    .Return(receive);
                return this.scenario;
            }
        }

        public class WaitContext
        {
            private readonly SerialPortScenario scenario;
            private readonly byte[] messageTypes;

            public WaitContext(SerialPortScenario scenario, byte[] messageTypes)
            {
                this.scenario = scenario;
                this.messageTypes = messageTypes;
            }

            public SerialPortScenario AndReceives(params byte[] receive)
            {
                Expect.Call(this.scenario.port.GetIncomingMessageOfType(this.messageTypes))
                    .Return(receive);
                return this.scenario;
            }
        }
    }
}
