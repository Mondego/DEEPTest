using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RemoteAssertionMessages;

namespace DeepTestFramework
{
    public class SystemUnderTestDeploymentAPI
    {
        public SystemUnderTestMessageHandler Messenger { get; private set; }

        public SystemUnderTestDeploymentAPI()
        {
            Messenger = new SystemUnderTestMessageHandler();
        }

        public SystemProcessWrapperWithInput ExecutePath(string path)
        {
            return ExecuteWithArguments(path, "");
        }

        public SystemProcessWrapperWithInput ExecuteWithArguments(string path, string arguments)
        {
            return new SystemProcessWrapperWithInput(path, arguments);
        }

        public object captureValue(InstrumentationPoint ip)
        {
            using (UdpClient client = new UdpClient(0))
            {
                IPEndPoint responseEndPoint = new IPEndPoint(IPAddress.Any, 0);
                InstrumentationPointExchangeMessage requestContents = new InstrumentationPointExchangeMessage
                {
                    instrumentationPointName = ip.Name,
                    instrumentationPointType = ip.GetType().ToString(),
                    value = ""
                };
                byte[] requestData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestContents));
                client.Send(requestData, requestData.Length, "127.0.0.1", RemoteTestingWrapper.RemoteConfigurations.RemoteListenerPort);

                while (client.Available <= 0) {}

                byte[] encoded = client.Receive(ref responseEndPoint);
                InstrumentationPointExchangeMessage response = 
                    JsonConvert.DeserializeObject<InstrumentationPointExchangeMessage>(
                        Encoding.UTF8.GetString(encoded));

                return response.value;
            }
        }
    }
}

