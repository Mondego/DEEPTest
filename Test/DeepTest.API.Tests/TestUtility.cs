using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DeepTest.API.Tests
{
    public static class TestUtility
    {
        public static string getRelativeSolutionPath(string testDirectory)
        {
            return Directory.GetParent(testDirectory).Parent.Parent.Parent.FullName;
        }

        public static void mockUdpClientRequest(
            string serverHostname, 
            int serverPort, 
            string message
        )
        {
            UdpClient client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));

            byte[] datagram = Encoding.UTF8.GetBytes(message);
            client.Send(datagram, datagram.Length, serverHostname, serverPort);
             
            client.Close();
        }
    }
}

