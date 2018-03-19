using System;
using System.Collections.Generic;

using Mono.Cecil.Cil;

namespace DeepTestFramework
{
    public class StopwatchHelper : InstrumentationHelper
    {
        public StopwatchHelper(InstrumentationAPI i) : base(i)
        {
        }

        protected override void InstrumentationHelperInitialization(
            InstrumentationPoint ip
        )
        {
            // TODO Inserts FieldDefinition for stopwatch
        }

        protected override List<Instruction> InstrumentationHelperOpeningInstructions(
            InstrumentationPoint ip
        )
        {
            List<Instruction> weaveOpeningInstructions = new List<Instruction>();

            // TODO Stopwatch.startnew

            return weaveOpeningInstructions;
        }

        protected override List<Instruction> InstrumentationHelperClosingInstructions(
            InstrumentationPoint ip
        )
        {
            List<Instruction> weaveClosingInstructions = new List<Instruction>();

            // TODO Stopwatch.Stop/Reset

            return weaveClosingInstructions;
        }
    }
}

