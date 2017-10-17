using System;
using System.Reflection;
using Mono.Cecil;
using System.IO;

namespace FlowTest
{
    public class InstrumentationHookHandler
    {
        public AssemblyDefinition customHooksModule { get; }

        public InstrumentationHookHandler()
        {
            string workingDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            string instrumentationHookPath = workingDirectory + "/FlowTestInstrumentation.dll";
            
            customHooksModule = AssemblyDefinition.ReadAssembly(instrumentationHookPath);
        }
    }
}

