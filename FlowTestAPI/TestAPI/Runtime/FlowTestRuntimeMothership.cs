using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace FlowTestAPI
{	
	// Cheesy name, handles the nitty-gritty of endpoints, content messages, etc 
	public class FlowTestRuntimeMothership
	{
		private FlowTestRuntimeConnection MothershipEndpoint;
		private FlowTestRuntimeConnection AwayTeamComponentEndpoint;
		private TcpListener MothershipMessageListener;

		// TODO - Shouldn't be hardcoded
		private int defaultMothershipEndpointPort = 60011;
		private int defaultAwayTeamEndpointPort = 60012;

		// Default for now is we're just going to weave stuff into one target component per test runtime.
	    // e.g., a mapping of one NUnit test suite to one FlowTestRuntime, and consequently one target
		// component. No idea if this is accurate long-term but seems organized enough.
		public FlowTestRuntimeMothership (int quantityTargetComponents = 1)
		{
			MothershipEndpoint = new FlowTestRuntimeConnection (IPAddress.Any, defaultMothershipEndpointPort);
			AwayTeamComponentEndpoint = new FlowTestRuntimeConnection (IPAddress.Any, defaultAwayTeamEndpointPort);

			MothershipMessageListener = new TcpListener(MothershipEndpoint.Address, MothershipEndpoint.Port);
		}

		public object GetPropertyOfInterest (string propertyPath)
		{
			MothershipMessageListener.Start ();

			PropertyOfInterestRequestPayload getPOI = new PropertyOfInterestRequestPayload
			{
				poiType = null,
				poiPath = propertyPath,
				poiValue = null
			};

			Task.Run(() =>
				{
					Thread.Sleep(1000);

					TcpClient tcpc = new TcpClient("127.0.0.1", AwayTeamComponentEndpoint.Port);
					NetworkStream nsc = tcpc.GetStream();

					byte[] messageData = Encoding.ASCII.GetBytes(
						JsonConvert.SerializeObject(getPOI, Formatting.None));

					nsc.Write(messageData, 0, messageData.Length);
					nsc.Close();
					tcpc.Close();
				});

			TcpClient tc = MothershipMessageListener.AcceptTcpClient();
			NetworkStream ns = tc.GetStream();
			StreamReader sr = new StreamReader(ns);
			string receivedJSON = sr.ReadToEnd();

			Console.WriteLine("[DEBUG localhost:{0} received result]: {1}", MothershipEndpoint.Port, receivedJSON); 
			PropertyOfInterestRequestPayload deserializedReceivedPOI = 
				JsonConvert.DeserializeObject<PropertyOfInterestRequestPayload>(receivedJSON);

			tc.Close();
			ns.Close();
			sr.Close();

			MothershipMessageListener.Stop();

			return deserializedReceivedPOI.poiValue;
		}
	}
}

