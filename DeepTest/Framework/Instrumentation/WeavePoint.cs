using System;
using System.IO;
using Mono.Cecil;

namespace Framework
{
	public class WeavePoint
	{
        public MethodDefinition wpMethodDefinition { get; }

		public WeavePoint (
			string parentModule,
			string parentType,
			string methodToWatch,
            MethodDefinition methodDefinition
        )
        {
            wpMethodDefinition = methodDefinition;
        }

        public override string ToString()
        {
            return wpMethodDefinition.FullName;
        }
	}
}

