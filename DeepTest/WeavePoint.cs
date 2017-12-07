using System;
using System.IO;

namespace DeepTest
{
	public class WeavePoint
	{
		public string moduleReadPath { get; }
        public string parentNamespaceOfWatchpoint { get; }
		public string parentTypeOfWatchpoint { get; }
		public string methodOfInterest { get; }

		public WeavePoint (
			string parentModule,
            string parentNamespace,
			string parentType,
			string methodToWatch
		)
		{
			moduleReadPath = parentModule;
            parentNamespaceOfWatchpoint = parentNamespace;
			parentTypeOfWatchpoint = parentType;
			methodOfInterest = methodToWatch;
            Console.WriteLine(this.ToString());
		}

        public override string ToString()
        {
            return String.Format("[WP] {0} {1}.{2}.{3}", 
                new DirectoryInfo(moduleReadPath).Name, 
                parentNamespaceOfWatchpoint, 
                parentTypeOfWatchpoint, 
                methodOfInterest);
        }
	}
}

