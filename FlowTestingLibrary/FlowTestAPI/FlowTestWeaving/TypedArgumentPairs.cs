using System;
using Mono.Cecil.Cil;

namespace FlowTestAPI
{
	public class TypedArgumentPairs
	{
		public Type[] mTypes { get; set; }
		public OpCode[] mOpCodes { get; set; }
		public object[] mValues { get; set; }

		public TypedArgumentPairs (
			Type[] types,
			OpCode[] opcodes,
			object[] values
		)
		{
			mTypes = types;
			mOpCodes = opcodes;
			mValues = values;
		}
	}
}

