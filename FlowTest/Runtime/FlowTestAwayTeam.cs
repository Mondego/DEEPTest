using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FlowTest
{
	// Cheesy name for now, this component is woven into the target component for 
	// communication with the mothership (aka, the test runtime's messaging piece)
	public class FlowTestAwayTeam
	{
		private Dictionary<int, object> mEntanglements;
		private FlowTestRuntimeConnection AwayTeamConnection;
		private static FlowTestRuntimeConnection MothershipConnection;
		private TcpListener AwayTeamTCPListener;
		private bool isListeningForRuntimeRequests = false;

		// TODO - Shouldn't be hardcoded
		private int defaultMothershipConnectionPort = 60011;
		private int defaultAwayTeamConnectionPort = 60012;

		public FlowTestAwayTeam (int dumb1, int dumb2)
		{
			Console.WriteLine (
				"Deploying FlowTest away team to <localhost:{0}> from test mothership <localhost:{1}>", 
				defaultAwayTeamConnectionPort,
				defaultMothershipConnectionPort
			); 

			mEntanglements = new Dictionary<int, object> ();

			AwayTeamConnection = new FlowTestRuntimeConnection (IPAddress.Any, defaultAwayTeamConnectionPort);
			MothershipConnection = new FlowTestRuntimeConnection (IPAddress.Any, defaultMothershipConnectionPort);
		}

		public void Helper()
		{
			Console.WriteLine("Sanity check on FlowTestAwayTeam");
		}

		public static void SendRunTimeEvent(string serializedEvent)
		{
			TcpClient tcpc = new TcpClient("127.0.0.1", MothershipConnection.Port);
			NetworkStream ns = tcpc.GetStream();
			byte[] messageData = Encoding.ASCII.GetBytes(serializedEvent);
			ns.Write(messageData, 0, messageData.Length);
			ns.Close();
			tcpc.Close();
		}

		public static void SendRewrappedRunTimeEvent(string serializedEvent, object content = null)
		{
			if (content != null) {
				FlowTestInstrumentationEvent unwrap = 
					JsonConvert.DeserializeObject<FlowTestInstrumentationEvent> (serializedEvent);
			
				unwrap.flowEventContent = content;

				serializedEvent = JsonConvert.SerializeObject (unwrap, Formatting.None);
			}

			SendRunTimeEvent (serializedEvent);
		}

		public void InvokeCustomType(string typeToCreate, string methodToInvoke)
		{
			//Console.WriteLine ("INVOKING: " + methodToInvoke);
			//Activator.CreateInstance(Type.GetType(myStorageClassNameReadFromAppConfig);
		}
	}
}

