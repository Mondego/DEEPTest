using System;
using System.Reflection;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Inject;

namespace FlowTest
{
    public static class WeavingPrebuiltInstrumentation
    {
        public static void WeaveSendEvent(
            AssemblyDefinition instrumentationHooksModule,
            AssemblyDefinition weaveTargetModule,
            WeavePoint weavePoint
        )
        {
            try
            {
                // The Hook we're going to add
                MethodDefinition sendEventMethodHook = instrumentationHooksModule
                    .MainModule
                    .GetType("FlowTestInstrumentation.InstrumentationHooks")
                    .GetMethod("SendEvent");

                // Where it will be woven
                MethodDefinition weavePointTargetMethod = weaveTargetModule
                    .MainModule
                    .GetType(weavePoint.parentNamespaceOfWatchpoint + "." + weavePoint.parentTypeOfWatchpoint)
                    .GetMethod(weavePoint.methodOfInterest);
                
                InjectionDefinition injector = 
                    new InjectionDefinition(
                        injectTarget: weavePointTargetMethod,
                        injectMethod: sendEventMethodHook,
                        flags: InjectFlags.None
                    );
                injector.Inject();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message + " BLAH");
            }
        }
    }
}

