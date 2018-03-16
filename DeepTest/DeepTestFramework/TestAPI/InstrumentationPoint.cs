using System;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DeepTestFramework
{
    public class InstrumentationPoint
    {
        private InstrumentationAPI parent = null;
        public AssemblyDefinition instrumentationPointAssemblyDefinition { get; private set; }
        public TypeDefinition instrumentationPointTypeDefinition { get; private set; }
        public MethodDefinition instrumentationPointMethodDefinition { get; private set; }

        public string AssemblyName { get; private set; }
        public string Name { get; private set; }

        public InstrumentationPoint(string name)
        {
            Name = name;
            AssemblyName = "";
            instrumentationPointAssemblyDefinition = null;
            instrumentationPointTypeDefinition = null;
            instrumentationPointMethodDefinition = null;
        }

        public InstrumentationPoint(string name, InstrumentationAPI apiRoot)
        {
            parent = apiRoot;
            Name = name;
            AssemblyName = "";

            instrumentationPointAssemblyDefinition = null;
            instrumentationPointTypeDefinition = null;
            instrumentationPointMethodDefinition = null;
        }

        public InstrumentationPoint FindInAssemblyNamed(string assemblyName)
        {
            instrumentationPointAssemblyDefinition = parent.getAssemblyDefinitionByName(assemblyName);
            AssemblyName = assemblyName;

            return this;
        }

        public InstrumentationPoint FindInTypeNamed(string typeName)
        {
            instrumentationPointTypeDefinition = 
                instrumentationPointAssemblyDefinition.MainModule.Types
                    .Single(t => t.Name == typeName);

            return this;
        }

        public InstrumentationPoint FindMethodNamed(string methodName)
        {
            instrumentationPointMethodDefinition =
                instrumentationPointTypeDefinition.Methods.Single(m => m.Name == methodName);

            return this;
        }

        public void printMethodInstructions()
        {
            Console.WriteLine("/// {0} ///", instrumentationPointMethodDefinition.FullName);
            if (instrumentationPointMethodDefinition != null) {
                foreach (Instruction i in instrumentationPointMethodDefinition.Body.Instructions) {
                    Console.WriteLine(i);
                }
            }
            Console.WriteLine("////////");
        }
    }
}

