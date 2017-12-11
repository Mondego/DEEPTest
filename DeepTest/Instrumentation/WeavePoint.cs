using System;
using System.IO;

namespace DeepTest
{
	public class WeavePoint
	{
		public string moduleReadPath { get; }
		public string parentTypeOfPoint { get; }
		public string methodOfInterest { get; }

		public WeavePoint (
			string parentModule,
			string parentType,
			string methodToWatch
		)
		{
			moduleReadPath = parentModule;
			parentTypeOfPoint = parentType;
			methodOfInterest = methodToWatch;
		}

        public override string ToString()
        {
            return String.Format("[WP] {0} {1}.{2}",
                new DirectoryInfo(moduleReadPath).Name,
                parentTypeOfPoint, 
                methodOfInterest);
        }
	}
}

