using System;
using System.IO;
using Mono.Cecil;

namespace DeepTest
{
	public class WeavePoint
	{
        public WeavePointSignature wpPath { get; }
        public MethodDefinition wpMethodDefinition { get; }

		public WeavePoint (
			string parentModule,
			string parentType,
			string methodToWatch,
            MethodDefinition methodDefinition
        )
        {
            wpPath = new WeavePointSignature(
                path: parentModule,
                typeName: parentType,
                methodName: methodToWatch
            );
            wpMethodDefinition = methodDefinition;
        }

        public override string ToString()
        {
            return wpPath.ToString();
        }
	}
}

