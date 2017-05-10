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

namespace FlowTestAPI
{
	// Cheesy name for now, this component is woven into the target component for 
	// communication with the mothership (aka, the test runtime's messaging piece)
	public class FlowTestAwayTeam
	{
		private Dictionary<int, object> mEntanglements;
		private FlowTestRuntimeConnection AwayTeamConnection;
		private FlowTestRuntimeConnection MothershipConnection;
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

		public void SendRunTimeEvent(string serializedEvent)
		{
			TcpClient tcpc = new TcpClient("127.0.0.1", MothershipConnection.Port);
			NetworkStream ns = tcpc.GetStream();
			byte[] messageData = Encoding.ASCII.GetBytes(serializedEvent);
			ns.Write(messageData, 0, messageData.Length);
			ns.Close();
			tcpc.Close();
		}

		public void SendRewrappedRunTimeEvent(string serializedEvent, object content = null)
		{
			if (content != null) {
				FlowTestInstrumentationEvent unwrap = 
					JsonConvert.DeserializeObject<FlowTestInstrumentationEvent>(serializedEvent);
			
				unwrap.flowEventContent = content;

				serializedEvent = JsonConvert.SerializeObject(unwrap, Formatting.None);
			}

			SendRunTimeEvent(serializedEvent);
		}

		/* LEGACY

		private void Run()
		{
			AwayTeamTCPListener = new TcpListener(AwayTeamConnection.Address, AwayTeamConnection.Port);
			isListeningForRuntimeRequests = true;

			Task.Run(() =>
				{
					try
					{
						AwayTeamTCPListener.Start();
						while (isListeningForRuntimeRequests)
						{
							// Listen for a property of interest request from the test runtime mothership
							TcpClient tc = AwayTeamTCPListener.AcceptTcpClient();
							NetworkStream ns = tc.GetStream();
							StreamReader sr = new StreamReader(ns);

							string propertyRequestJson = sr.ReadToEnd();
							FlowTestInstrumentationEvent poiRequestMessageContents = 
								JsonConvert.DeserializeObject<FlowTestInstrumentationEvent>(propertyRequestJson);
							Console.WriteLine(
								"[Message received by Test Runtime Away Team at localhost:{0}]: {1}", 
								AwayTeamConnection.Port, 
								propertyRequestJson
							); 

							ns.Close();
							sr.Close();
							tc.Close();

							// TODO begin super basic search for a matching type and appropriate nested values
							object poiValueToReturn = null;
							string valueRequestPath = poiRequestMessageContents.flowInstrumentationPath;

							List<string> propertiesOfRequestedObject = new List<string>(valueRequestPath.Split(new char[] {'.'}));

							string targetType = propertiesOfRequestedObject[0];
							propertiesOfRequestedObject.RemoveAt(0);

							object registeredObjectContainingTargetValue = 
								mEntanglements.Values.SingleOrDefault(r => r.GetType().Name == targetType);

							if (registeredObjectContainingTargetValue != null)
							{
								poiValueToReturn = searchForNestedValue(
									targetObject: registeredObjectContainingTargetValue,
									fieldsToSearchInOrder: propertiesOfRequestedObject
								);
							}

							else
							{
								Console.WriteLine(
									"Could not find any registered objects of type {0}",
									targetType
								);
								poiValueToReturn = null;
							}
							// end super basic search for a matching type and appropriate nested values

							FlowTestInstrumentationEvent responseMessage = new FlowTestInstrumentationEvent
							{
								flowParentType = null,
								flowInstrumentationPath = poiRequestMessageContents.flowInstrumentationPath,
								sourceFlowKey = poiRequestMessageContents.sourceFlowKey,
								flowEventContent = poiValueToReturn
							};

							// Send the result back...
							TcpClient tcpc = new TcpClient("127.0.0.1", MothershipConnection.Port);
							ns = tcpc.GetStream();
							byte[] messageData = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(responseMessage, Formatting.None));
							ns.Write(messageData, 0, messageData.Length);
							ns.Close();
							tcpc.Close();
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("Test Runtime AwayMessage caught exception: \n" + e.Message);
					}
					finally
					{
						isListeningForRuntimeRequests = false;
						AwayTeamTCPListener.Stop();
					}
				});
		}

		public void EntangleWithLocalTestRuntime(object wovenComponent)
		{
			Console.WriteLine("Test Away Team running at <localhost:{0}> has landed on woven component {1}",
				AwayTeamConnection.Port,
				wovenComponent.GetType());
		
			// TODO holdover from old code, really not the best way to do this.
			mEntanglements.Add(wovenComponent.GetHashCode(), wovenComponent);
		}
			
		// TODO should we move this
		private object searchForNestedValue(object targetObject, List<string> fieldsToSearchInOrder)
		{
			var currentSource = targetObject;
			var currentValue = targetObject;
			object returnValue = null;

			foreach(string nestedProperty in fieldsToSearchInOrder)
			{
				Console.WriteLine("Looking for nested property {0} in object of type {1}",
					nestedProperty, currentSource.GetType());

				var currentField = currentSource.GetType().GetField(nestedProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

				if (currentField == null)
				{
					var tryProperty = currentSource.GetType().GetProperty(nestedProperty, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

					if (tryProperty == null)
					{
						Console.WriteLine("Field {0} was null", nestedProperty);
						return null;
					} else
					{
						currentValue = tryProperty.GetValue(currentSource);
					}
				} 

				else
				{
					currentValue = currentField.GetValue(currentSource);
				}

				currentSource = currentValue;
			}

			return currentValue;
		}*/
	}
}

