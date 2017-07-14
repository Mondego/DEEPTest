using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FlowTest
{
	public class WeavingDebugTools
	{
		public static void ConsoleWriteEachCILInstruction(MethodDefinition methodToPrint)
		{
			Console.WriteLine ("v---- Instructions in {0} ----v", methodToPrint.FullName);
			ILProcessor instructionProcessor = methodToPrint.Body.GetILProcessor();

			foreach(Instruction ii in instructionProcessor.Body.Instructions) {
				Console.WriteLine(ii);
			}
			Console.WriteLine ("^---- End Instructions in {0} ----^", methodToPrint.FullName);
		}

		public static string GetStandaloneFTFieldName(string TypeName)
		{
			return "_FT_" + TypeName;
		}
	}
}

