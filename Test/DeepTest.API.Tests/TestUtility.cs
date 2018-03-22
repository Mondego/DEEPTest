using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DeepTest.API.Tests
{
    public static class TestUtility
    {
        public static string getRelativeSolutionPath(string testDirectory)
        {
            return Directory.GetParent(testDirectory).Parent.Parent.Parent.FullName;
        }

        public static void mockUdpClientMessageRequest(
            string serverHostname, 
            int serverPort, 
            string message
        )
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            byte[] datagram = Encoding.UTF8.GetBytes(message);
            client.Send(datagram, datagram.Length, serverHostname, serverPort);
             
            while (client.Available <= 0)
            {
            }
            Thread.Sleep(1000);

            client.Close();
        }
    }
}

