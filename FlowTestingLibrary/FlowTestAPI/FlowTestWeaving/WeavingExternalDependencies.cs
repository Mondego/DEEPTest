using System;
using Mono.Cecil;

namespace FlowTestAPI
{
	public class WeavingExternalDependencies
	{
		public static void importDependencies(
			ModuleDefinition moduleToWeave,
			Type dependencyToImport
		)
		{
			Console.WriteLine("Known assembly references: ");
			foreach(AssemblyNameReference anr in moduleToWeave.AssemblyReferences)
			{
				Console.WriteLine(anr.FullName);
			}
		}
	}
}

