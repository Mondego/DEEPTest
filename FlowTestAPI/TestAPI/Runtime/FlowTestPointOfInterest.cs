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

		public bool watchBefore = false;
		public bool watchAfter = false;

		public FlowTestPointOfInterest (string pointOfInterestPath)
		{
			pointOfInterest = pointOfInterestPath;
		}

		// TODO
		public void Before()
		{
			Console.WriteLine ("TODO placeholder - point of interest [Before]");
			watchBefore = true;
		}

		// TODO
		public void After()
		{
			Console.WriteLine ("TODO placeholder - point of interest [After]");
			watchBefore = true;
		}
	}
}

