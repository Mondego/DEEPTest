using System;

namespace FlowTestAPI
{
	public class FlowTestPointOfInterest
	{
		private string pointOfInterest;
		public string PointOfInterest
		{
			get { return pointOfInterest; }
		}

		public FlowTestPointOfInterest (string pointOfInterestPath)
		{
			pointOfInterest = pointOfInterestPath;
		}

		// TODO
		public void Before()
		{
			Console.WriteLine ("TODO placeholder - point of interest [Before]");
		}

		// TODO
		public void After()
		{
			Console.WriteLine ("TODO placeholder - point of interest [After]");
		}
	}
}

