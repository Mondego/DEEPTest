using System;
using System.Collections.Generic;
using System.Linq;

using Mono.Cecil;
using Mono.Cecil.Cil;

using DeepTestWrapper;

namespace DeepTest
{
    public class WeavingHandler
    {
        private Dictionary<string, AssemblyDefinition> mWeaves;

        public WeavingHandler()
        {
            mWeaves = new Dictionary<string, AssemblyDefinition>();
        }
            
        public WeavePoint AddWeavePointToNode(
            DTNodeDefinition target,
            string nameOfWeavePointType,
            string nameOfWeavePointMethod
        )
        {
            try
            {
                AssemblyDefinition wpDestination;
                mWeaves.TryGetValue(target.readPath, out wpDestination);

                MethodDefinition wpMethod = findMethodDefinition(
                    wpDestination,
                    nameOfWeavePointType,
                    nameOfWeavePointMethod
                );

                WeavePoint wp = new WeavePoint(
                    target.readPath,
                    nameOfWeavePointType,
                    nameOfWeavePointMethod,
                    wpMethod
                );
                
                //addWeavePointAnchor(wpDestination, wp);

                StopwatchHelper.addStopwatchInWeavePoint(
                    wp,
                    wp.wpMethodDefinition.Body.Instructions.First(),
                    wp.wpMethodDefinition.Body.Instructions.Last().Previous
                );

                Console.WriteLine("***");
                foreach(Instruction i in wp.wpMethodDefinition.Body.Instructions)
                {
                    Console.WriteLine(i);
                }
                Console.WriteLine("***");

                return wp;
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.AddWeavePointToNode caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }

            return null;
        }

        public MethodDefinition findMethodDefinition(
            AssemblyDefinition ad,
            string nameOfType,
            string nameOfMethod
        )
        {
            try
            {
                TypeDefinition foundTypeDefinition = ad.MainModule.Types.Single(td => td.Name == nameOfType);
                MethodDefinition foundMethodDefinition = foundTypeDefinition.Methods.Single(md => md.Name == nameOfMethod);

                return foundMethodDefinition;
            }

            catch (Exception e) {
                Console.WriteLine("WeavingHandler.findMethodDefinition caught exception {0}",
                    e.Message);
            }

            return null;
        }

        /// <summary>
        /// Adds anchor for assertions at entrance and exit of target method
        /// </summary>
        /// <param name="ad">AssemblyDefinition to be woven</param>
        /// <param name="wp">WeavePoint to use for metadata</param>
        public void addWeavePointAnchor(AssemblyDefinition ad, WeavePoint wp)
        {
            try
            {
                List<Instruction> weaveInstructions = new List<Instruction>();

                weaveInstructions.Add(
                    wp.wpMethodDefinition.Body.GetILProcessor().Create(
                        OpCodes.Callvirt,
                        wp.wpMethodDefinition.Module.Import(
                            typeof(DTWrapper).GetMethod("Assert", new Type[] {}))));

                MethodDefinition sendMsgDebug = findMethodDefinition(ad, wp.wpPath.wpContainingTypeName, "SendMessageCallback");

                WeaveDebugInfoAtWeavePointExit(wp, "info: weaving DTWrapper @ EchoChatServer.ReceiveMessageCallback");
            
                WeavingAspectLocation.WeaveInstructions(
                    WeavingAspectLocation.WeaveLocation.MethodEntry | WeavingAspectLocation.WeaveLocation.MethodExit,
                    wp,
                    weaveInstructions
                );

                WeaveDebugInfoAtWeavePointEntry(wp, "info: debugging DTWrapper @ EchoChatServer.ReceiveMessageCallback");
            }

            catch (Exception e)
            {
                Console.WriteLine("WeavingHandler.addWeavePointAnchor caught exception " + e.Message);
            }
        }

        public void WeaveDebugInfoAtWeavePointEntry(WeavePoint wp, string info = "")
        {
            List<Instruction> weaveInstructions = new List<Instruction>();
            string loadEntryValue = "[Debug] Weaving Before " + info;
        
            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(OpCodes.Ldstr, loadEntryValue)
            );
            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(
                    OpCodes.Call,
                    wp.wpMethodDefinition.Module.Import(
                        typeof(System.Console).GetMethod("WriteLine", new [] { typeof(string) }))));

            WeavingAspectLocation.WeaveInstructions(
                WeavingAspectLocation.WeaveLocation.MethodEntry,
                wp,
                weaveInstructions
            );
        }

        public void WeaveDebugInfoAtWeavePointExit(WeavePoint wp, string info = "")
        {
            List<Instruction> weaveInstructions = new List<Instruction>();
            string loadExitValue = "[Debug] Weaving After " + info;

            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(OpCodes.Ldstr, loadExitValue)
            );
            weaveInstructions.Add(
                wp.wpMethodDefinition.Body.GetILProcessor().Create(
                    OpCodes.Call,
                    wp.wpMethodDefinition.Module.Import(
                        typeof(System.Console).GetMethod("WriteLine", new [] { typeof(string) }))));

            WeavingAspectLocation.WeaveInstructions(
                WeavingAspectLocation.WeaveLocation.MethodExit,
                wp,
                weaveInstructions
            );
        }

        public void ReadAssembly(string path)
        {
            try
            {
                if (!mWeaves.ContainsKey(path))
                {
                    AssemblyDefinition m = AssemblyDefinition.ReadAssembly(path);
                    mWeaves.Add(path, m);
                }
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.AddWeavePointToNode caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }

        public void Write(DTNodeDefinition target, string alternateWritePath = "")
        {
            try
            {
                string writePath = target.readPath;
                if (alternateWritePath != "")
                {
                    writePath = alternateWritePath;
                }

                AssemblyDefinition assemblyToWrite = mWeaves[target.readPath];
                assemblyToWrite.Write(writePath);
            }

            catch (Exception e) {
                Console.WriteLine(
                    "DTWeavingHandler.Write() caught unexpected {0} {1}",
                    e.GetType(),
                    e.Message);
            }
        }
    }
}

