using System;
using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace DeepTestFramework
{
    public class SnapshotLocalValueHelper : InstrumentationHelper
    {
        public SnapshotLocalValueHelper(InstrumentationAPI i) : base(i)
        {
        }

        protected override void InstrumentationHelperInitialization(
            InstrumentationPoint ip
        )
        {
            // TODO will likely need more initialization to retrieve a value on demand
            base.InstrumentationHelperInitialization(ip);
        }

        protected override List<Instruction> InstrumentationHelperOpeningInstructions(
            InstrumentationPoint ip
        )
        {
            List<Instruction> weaveOpeningInstructions = new List<Instruction>();

            // TODO calls "this" then loads value of interest

            return weaveOpeningInstructions;
        }
    }
}

