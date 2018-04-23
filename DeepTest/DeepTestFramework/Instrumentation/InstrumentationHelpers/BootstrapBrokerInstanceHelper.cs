using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using RemoteTestingWrapper;

namespace DeepTestFramework
{
    public class BootstrapBrokerInstanceHelper : InstrumentationHelper
    {
        public BootstrapBrokerInstanceHelper(InstrumentationAPI i) : base(i)
        {
        }

        protected override List<Instruction> InstrumentationHelperOpeningInstructions(
            InstrumentationPoint ip
        )
        {
            List<Instruction> weaveOpeningInstructions = new List<Instruction>();

            ip.instrumentationPointMethodDefinition.Body.SimplifyMacros();
            ILProcessor ilp = ip.instrumentationPointMethodDefinition.Body.GetILProcessor();

            // Call default remote test wrapper handler
            Instruction loadRemoteTestDriverInstance =
                ilp.Create(OpCodes.Call, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(StandaloneInstrumentationMessageHandler).GetMethod("get_Instance", new Type[] { })));

            Instruction callInstanceBootstrap =
                ilp.Create(OpCodes.Callvirt, 
                    ip.instrumentationPointMethodDefinition.Module.Import(
                        typeof(StandaloneInstrumentationMessageHandler).GetMethod(
                            "Bootstrap", new Type[] { })));

            ip.instrumentationPointMethodDefinition.Body.OptimizeMacros();

            weaveOpeningInstructions.Add(loadRemoteTestDriverInstance);
            weaveOpeningInstructions.Add(callInstanceBootstrap);

            return weaveOpeningInstructions;
        }
    }
}

