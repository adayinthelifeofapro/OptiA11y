using System;
using System.Collections.Generic;
using EPiServer.Shell.ObjectEditing.Internal;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       NOTE: This is a pre-release API that is UNSTABLE and might not satisfy the compatibility requirements as denoted by its associated normal version.
///
///       The combination of <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.DataType" /> and <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.UIHint" /> will be the
///       unique identifier to determine for what type this definition will apply to.
///       </summary>
public class EditorDefinition
{
	/// <summary>
	///       The type that this editor definition should affect.
	///       </summary>
	public Type DataType { get; }

	/// <summary>
	///       The type of the value for this editor definition.
	///       </summary>
	public Type ValueType { get; set; }

	/// <summary>
	///       What UIHint should the editor definition affect. Empty string will affect default editors of the specified <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.DataType" />.
	///       </summary>
	public string UIHint { get; }

	/// <summary>
	///       What editor widget to use.
	///       </summary>
	public string Editor { get; set; }

	/// <summary>
	///       The settings to apply to the editor widget.
	///       </summary>
	public IDictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> class.
	///       </summary>
	/// <param name="dataType">
	/// </param>
	/// <param name="uiHint">
	/// </param>
	public EditorDefinition(Type dataType, string uiHint = "")
	{
		DataType = dataType;
		UIHint = uiHint ?? "";
	}

	internal EditorDefinitionDDSO ToEditorDefinitionDDSO()
	{
		return new EditorDefinitionDDSO
		{
			DataTypeString = TypeSerializer.GetAssemblyTypeNameWithoutVersion(DataType),
			ValueTypeString = (((object)ValueType == null) ? null : TypeSerializer.GetAssemblyTypeNameWithoutVersion(ValueType)),
			UIHint = UIHint,
			Editor = Editor,
			Settings = Settings
		};
	}
}
