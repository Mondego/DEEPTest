using System;
using System.Net;

namespace FlowTestAPI
{
	public class FlowTestRuntimeConnection
	{
		private IPAddress endpointAddress;
		private int endpointPort;

		public IPAddress Address
		{
			get { return endpointAddress; }
		}

		public int Port
		{
			get { return endpointPort; }
		}

		public FlowTestRuntimeConnection (IPAddress address, int port)
		{
			endpointAddress = address;
			endpointPort = port;
		}
	}
}

