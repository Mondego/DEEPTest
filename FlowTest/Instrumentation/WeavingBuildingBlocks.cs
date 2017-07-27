using System;
using System.Linq;

using Mono.Cecil;

namespace FlowTest
{
	public class WeavingBuildingBlocks
	{
		#region Fields

		public static void _AddFieldDefinitionToType
		(
			ModuleDefinition moduleToWeave,
			string nameOfDestinationType,
			string nameOfField,
			FieldAttributes attributesOfField,
			TypeReference typeReferenceOfField
		)
		{
			try {
				TypeDefinition destinationType = moduleToWeave.Types.Single (t => t.Name == nameOfDestinationType);

				FieldDefinition addingField = new FieldDefinition (
					name: nameOfField,
					attributes: attributesOfField,
					fieldType: typeReferenceOfField
				);

				destinationType.Fields.Add(addingField);
			}

			catch (Exception ex) {
				Console.WriteLine("Exception in WeavingBuildingBlocks.AddFieldDefinitionToTypeDefinition {0} {1}",
					ex.InnerException,
					ex.Message);
			}
		}

		public static void WeavePublicStaticFieldHelper(
			ModuleDefinition module,
			string typeName,
			string fieldName,
			Type typeOfField
		)
		{
			_AddFieldDefinitionToType(
				moduleToWeave: module,
				nameOfDestinationType: typeName,
				nameOfField: fieldName,
				attributesOfField: FieldAttributes.Public | FieldAttributes.Static,
				typeReferenceOfField: module.Import(typeOfField)
			);
		}

		#endregion
	}
}

