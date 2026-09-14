using System;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.ObjectEditing.Internal;

/// <summary>
///       Editor descriptor that will create an input to be able to edit a property based on an EditorDefinition
///       </summary>
internal class EditorDefinitionMetadataExtender : IMetadataExtender, IMetadataHandler
{
	private readonly EditorDefinition _editorDefinition;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.Internal.EditorDefinitionMetadataExtender" /> class.
	///       </summary>
	public EditorDefinitionMetadataExtender(EditorDefinition editorDefinition)
	{
		_editorDefinition = editorDefinition;
	}

	/// <summary>
	///       Use to add change metadata for the editor before the UI is rendered.
	///       </summary>
	/// <param name="metadata">The metadata.</param>
	/// <param name="attributes">The attributes.</param>
	public void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
	{
		if (!string.IsNullOrEmpty(_editorDefinition.Editor))
		{
			metadata.ClientEditingClass = _editorDefinition.Editor;
		}
		foreach (KeyValuePair<string, object> setting in _editorDefinition.Settings)
		{
			metadata.EditorConfiguration[setting.Key] = setting.Value;
		}
		if (!metadata.EditorConfiguration.TryGetValue("selections", out var value))
		{
			return;
		}
		if (value is IEnumerable<KeyValuePair<string, object>> source)
		{
			value = source.Select((KeyValuePair<string, object> x) => new SelectItem
			{
				Text = x.Key,
				Value = x.Value
			}).ToList();
			metadata.EditorConfiguration["selections"] = value;
		}
		if (value is IEnumerable<ISelectItem>)
		{
			metadata.SelectionFactoryType = typeof(EditorDefinitionSelectionFactory);
			if (string.IsNullOrWhiteSpace(metadata.ClientEditingClass))
			{
				metadata.ClientEditingClass = "epi-cms/contentediting/editors/SelectionEditor";
			}
		}
	}
}
