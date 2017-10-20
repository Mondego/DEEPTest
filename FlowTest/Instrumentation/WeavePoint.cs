using System;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Cil;
using System.Collections;
using System.IO;

namespace FlowTest
{
	public class WeavePoint
	{
		public string moduleReadPath { get; }
		public string parentTypeOfWatchpoint { get; }
        public string parentNamespaceOfWatchpoint { get; }
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
            Console.WriteLine("[New Weavepoint #{4}] {0} {1}.{2}.{3}", new DirectoryInfo(moduleReadPath).Name, parentNamespaceOfWatchpoint, parentTypeOfWatchpoint, methodOfInterest, this.GetHashCode());
		}
            
		public string generatePayload(string content = "")
		{
			List<KeyValuePair<string, string>> eventContents = new List<KeyValuePair<string, string>>();

			eventContents.Add(new KeyValuePair<string, string>("key", this.GetHashCode().ToString()));
			eventContents.Add(new KeyValuePair<string, string>("parentModuleName", moduleReadPath));
			eventContents.Add(new KeyValuePair<string, string>("parentTypeName", parentTypeOfWatchpoint));
			eventContents.Add(new KeyValuePair<string, string>("point", methodOfInterest));
			eventContents.Add(new KeyValuePair<string, string>("value", content));

			return new FormUrlEncodedContent(eventContents).ReadAsStringAsync().Result;
		}
	}
}

