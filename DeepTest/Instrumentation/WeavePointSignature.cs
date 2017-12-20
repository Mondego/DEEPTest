using System;
using System.IO;

namespace DeepTest
{
    public class WeavePointSignature
    {
        public string wpModuleReadPath { get; }
        public string wpContainingTypeName { get; }
        public string wpMethodName { get; }

        public WeavePointSignature
        (
            string path,
            string typeName,
            string methodName
        )
        {
            wpModuleReadPath = path;
            wpContainingTypeName = typeName;
            wpMethodName = methodName;
        }

        public override string ToString()
        {
            return String.Format("[DT.Inst.WPSignature] {0} {1}.{2}",
                new DirectoryInfo(wpModuleReadPath).Name,
                wpContainingTypeName, 
                wpMethodName
            );
        }
    }
}

