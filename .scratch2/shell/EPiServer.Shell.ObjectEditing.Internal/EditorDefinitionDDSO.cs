using System;
using System.Collections.Generic;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace EPiServer.Shell.ObjectEditing.Internal;

/// <summary>
///        Editor Definition Dynamic Data Store Object
///
///        The object that we persist in DDS for our <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> API.
///        </summary>
[EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
internal class EditorDefinitionDDSO : IDynamicData
{
	/// <summary>
	///       The Type and Assembly of the type that this editor definition should affect.
	///       Should be in the format of "TypeName,AssemblyName".
	///       This will be used to get the type after loading from DDS.
	///       </summary>
	[EPiServerDataIndex]
	public string DataTypeString { get; set; }

	/// <summary>
	///       Optional, the Type and Assembly of the value type that this editor definition should affect.
	///       Should be in the format of "TypeName,AssemblyName".
	///       This will be used to get the type after loading from DDS.
	///       </summary>
	[EPiServerDataIndex]
	public string ValueTypeString { get; set; }

	/// <summary>
	///       What UIHint should the editor definition affect. Empty string will affect default editors of the specified <see cref="P:EPiServer.Shell.ObjectEditing.Internal.EditorDefinitionDDSO.DataTypeString" />.
	///       </summary>
	[EPiServerDataIndex]
	public string UIHint { get; set; } = string.Empty;

	/// <summary>
	///       What editor widget to use.
	///       </summary>
	public string Editor { get; set; }

	/// <summary>
	///       The settings to apply to the editor widget.
	///       </summary>
	public IDictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();

	/// <summary>
	///       Dynamic data store identity.
	///       </summary>
	public Identity Id { get; set; }

	/// <summary>
	///       Converts this into a <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" />.
	///       Throw an exception if a type cannot be found for the given <see cref="P:EPiServer.Shell.ObjectEditing.Internal.EditorDefinitionDDSO.DataTypeString" /></summary>
	/// <returns>
	/// </returns>
	public EditorDefinition ToEditorDefinition()
	{
		Type type = TypeResolver.GetType(DataTypeString, true);
		Type valueType = (string.IsNullOrWhiteSpace(ValueTypeString) ? null : TypeResolver.GetType(ValueTypeString, false));
		return new EditorDefinition(type, UIHint)
		{
			ValueType = valueType,
			Editor = Editor,
			Settings = Settings
		};
	}
}
