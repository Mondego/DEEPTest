using System;
using System.Diagnostics;

namespace FlowTestInstrumentation
{
    public class WeaveAsserts {
        
        public static void WeaveAssert ()
        {
            Debug.Assert(
                condition: ??,
                message: ??
            );
        }
    }
}

