using System;
using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace DeepTestFramework
{
    public class SleepHelper : InstrumentationHelper
    {
        public SleepHelper(InstrumentationAPI i) : base(i)
        {
        }

        protected override List<Instruction> InstrumentationHelperOpeningInstructions(
            InstrumentationPoint ip
        )
        {
            List<Instruction> weaveOpeningInstructions = new List<Instruction>();

            // TODO load how many sleep seconds needed
            // TODO inject a thread.sleep call

            return weaveOpeningInstructions;
        }
    }
}

