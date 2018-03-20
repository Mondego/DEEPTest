using System;
using System.Collections.Generic;

using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace DeepTestFramework
{
    public abstract class InstrumentationHelper
    {
        protected InstrumentationAPI instrumentationAPI;

        public InstrumentationHelper(InstrumentationAPI instrumentation)
        {
            instrumentationAPI = instrumentation;
        }

        public InstrumentationHelper StartingAtEntry(InstrumentationPoint ip)
        {
            InstrumentationHelperInitialization(ip);
            List<Instruction> openingInstructions = InstrumentationHelperOpeningInstructions(ip);
            InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodEntry(ip, openingInstructions);  
            return _Save(ip);
        }

        public InstrumentationHelper StartingAtExit(InstrumentationPoint ip)
        {
            InstrumentationHelperInitialization(ip);
            List<Instruction> openingInstructions = InstrumentationHelperOpeningInstructions(ip);
            InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodExit(ip, openingInstructions);  
            return _Save(ip);
        }

        public InstrumentationHelper UntilEntry(InstrumentationPoint ip)
        {
            List<Instruction> closingInstructions = InstrumentationHelperClosingInstructions(ip);
            InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodExit(ip, closingInstructions); 
            return _Save(ip);
        }

        public InstrumentationHelper UntilExit(InstrumentationPoint ip)
        {
            List<Instruction> closingInstructions = InstrumentationHelperClosingInstructions(ip);
            InstrumentationPositionInMethodHelper.WeaveInstructionsAtMethodExit(ip, closingInstructions); 
            return _Save(ip);
        }

        protected virtual void InstrumentationHelperInitialization (InstrumentationPoint ip) 
        {
        }

        protected abstract List<Instruction> InstrumentationHelperOpeningInstructions(InstrumentationPoint ip);

        protected virtual List<Instruction> InstrumentationHelperClosingInstructions(InstrumentationPoint ip)
        {
            return new List<Instruction>();
        }

        protected InstrumentationHelper _Save(InstrumentationPoint ip)
        {
            instrumentationAPI.updateAssembly(ip);
            instrumentationAPI.writeAssembly(ip);

            return this;
        }
    }
}

