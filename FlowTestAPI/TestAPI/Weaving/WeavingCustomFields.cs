﻿using System;
using Mono.Cecil;
using System.Linq;

namespace FlowTestAPI
{
	public class WeavingCustomFields
	{
		public static void WeavePublicStaticFieldIntoModuleEntry (
			ModuleDefinition m,
			string customFieldName,
			Type fieldType
		)
		{
			TypeDefinition moduleMainClassType = m.Types.Single(t => t.Name == "MainClass");
			FieldDefinition publicStaticField = new FieldDefinition(
				customFieldName,
				Mono.Cecil.FieldAttributes.Static | Mono.Cecil.FieldAttributes.Public,
				m.Import (fieldType));

			moduleMainClassType.Fields.Add (publicStaticField); 
		}
	}
}

