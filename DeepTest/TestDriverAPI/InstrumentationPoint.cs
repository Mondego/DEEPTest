using System;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace TestDriverAPI
{
    public class InstrumentationPoint
    {
        private InstrumentationAPI parent = null;
        private AssemblyDefinition instrumentationPointParentAssembly = null;
        private TypeDefinition instrumentationPointParentType = null;
        private MethodDefinition instrumentationPointMethod = null;

        public string Name { get; }

        public InstrumentationPoint(string name)
        {
            Name = name;
        }

        public InstrumentationPoint(string name, InstrumentationAPI apiRoot)
        {
            Name = name;
            parent = apiRoot;
        }

        public InstrumentationPoint FindInAssemblyNamed(string assemblyName)
        {
            instrumentationPointParentAssembly = parent.getAssemblyDefinitionByName(assemblyName);

            return this;
        }

        public InstrumentationPoint FindInTypeNamed(string typeName)
        {
            instrumentationPointParentType = 
                instrumentationPointParentAssembly.MainModule.Types
                    .Single(t => t.Name == typeName);

            return this;
        }

        public InstrumentationPoint FindMethodNamed(string methodName)
        {
            instrumentationPointMethod =
                instrumentationPointParentType.Methods.Single(m => m.Name == methodName);

            return this;
        }

        public void printMethodInstructions()
        {
            Console.WriteLine("/// {0} ///", instrumentationPointMethod.FullName);
            if (instrumentationPointMethod != null) {
                foreach (Instruction i in instrumentationPointMethod.Body.Instructions) {
                    Console.WriteLine(i);
                }
            }
            Console.WriteLine("////////");
        }
    }
}

