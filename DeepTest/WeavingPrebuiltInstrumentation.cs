using System;
using System.Reflection;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
//using Mono.Cecil.Inject;

namespace DeepTest
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
                /*
                // The Hook we're going to add
                MethodDefinition sendEventMethodHook = instrumentationHooksModule
                    .MainModule
                    .GetType("FlowTestInstrumentation.FlowTestProxyTemplate")
                    .GetMethod("SendEvent");

                // Where it will be woven
                TypeDefinition weavePointTargetType =  weaveTargetModule
                    .MainModule
                    .GetType(weavePoint.parentNamespaceOfWatchpoint + "." + weavePoint.parentTypeOfWatchpoint);
                MethodDefinition weavePointTargetMethod = weavePointTargetType.GetMethod(weavePoint.methodOfInterest);

                weavePointTargetMethod.InjectWith(
                    injectionMethod: sendEventMethodHook,
                    tag: weavePoint.GetHashCode(),
                    flags: InjectFlags.PassTag
                );*/
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}

