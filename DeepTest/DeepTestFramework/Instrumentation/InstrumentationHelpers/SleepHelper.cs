using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DeepTestFramework
{
    public class SleepHelper : InstrumentationHelper
    {
        protected int nSecondsSleep;
        
        public SleepHelper(InstrumentationAPI i, int sleepSeconds) : base(i)
        {
            nSecondsSleep = sleepSeconds;
        }

        protected override List<Instruction> InstrumentationHelperOpeningInstructions(
            InstrumentationPoint ip
        )
        {
            List<Instruction> weaveOpeningInstructions = new List<Instruction>();

            ILProcessor ilp = ip.instrumentationPointMethodDefinition.Body.GetILProcessor();

            Instruction loadIntSleepQuantity = ilp.Create(OpCodes.Ldc_I4, nSecondsSleep * 1000);
            Instruction loadCallThreadSleep = 
                ilp.Create(
                    OpCodes.Call,
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(System.Threading.Thread).GetMethod("Sleep", new Type[] { typeof(int) })));

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();

            weaveOpeningInstructions.Add(loadIntSleepQuantity);
            weaveOpeningInstructions.Add(loadCallThreadSleep);

            return weaveOpeningInstructions;
        }
    }
}

