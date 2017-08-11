using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FlowTest
{
	public class WeavingDebug
	{
		public static void PrintCILInstructionsInMethod(
			MethodDefinition methodToPrint
		)
		{
			Console.WriteLine ("v---- Instructions in {0} ----v", methodToPrint.FullName);
			Console.WriteLine("v---- Has {0} variables ", methodToPrint.Body.Variables.Count);
			ILProcessor instructionProcessor = methodToPrint.Body.GetILProcessor();

			foreach(Instruction ii in instructionProcessor.Body.Instructions) {
				Console.WriteLine(ii);
			}
			Console.WriteLine ("^---- End Instructions in {0} ----^", methodToPrint.FullName);
		}
	}
}

