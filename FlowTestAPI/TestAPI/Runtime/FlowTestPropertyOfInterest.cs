using System;

namespace FlowTestAPI
{
	public class FlowTestPropertyOfInterest
	{
		private string pathToPropertyOfInterest;
		public string Property
		{
			get { return pathToPropertyOfInterest; }
		}

		public FlowTestPropertyOfInterest (string pathToProperty)
		{
			pathToPropertyOfInterest = pathToProperty;
		}

		public object GetPropertyFromRuntime (FlowTestRuntime test)
		{
			return test.GetPropertyOfInterest (pathToPropertyOfInterest);
		}
	}
}

