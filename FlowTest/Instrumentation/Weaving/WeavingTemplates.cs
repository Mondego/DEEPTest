using System;
using Mono.Cecil;
using System.Linq;

namespace FlowTest
{
	public static class WeavingTemplates
	{
		public static ModuleDefinition Templates = 
			ModuleDefinition.ReadModule(typeof(FlowTestTemplates.CopyCat).Assembly.Location);

		public static TypeDefinition getTypeTemplate(string typeName)
		{
			return Templates.Types.SingleOrDefault(t => t.Name == typeName);
		}

		public static MethodDefinition getMethodInType(
			string typeName,
			string methodName
		)
		{
			return getTypeTemplate(typeName).Methods.SingleOrDefault(m => m.Name == methodName);
		}

		public static MethodDefinition getStub(string stubName)
		{
			return getMethodInType(
				typeName: "Stubs",
				methodName: stubName
			);
		}
	}
}

