using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace EPiServer.Shell.ObjectEditing.Internal;

internal static class EditorDefinitionDDSOExtensions
{
	private static string SerializeSelections(EditorDefinitionSelections selections)
	{
		return JsonSerializer.Serialize(selections);
	}

	private static string SerializeSelections(IEnumerable<KeyValuePair<string, object>> selections)
	{
		return SerializeSelections(new EditorDefinitionSelections(selections));
	}

	private static string SerializeSelections(object value)
	{
		if (value is EditorDefinitionSelections selections)
		{
			return SerializeSelections(selections);
		}
		if (value is IEnumerable<KeyValuePair<string, object>> selections2)
		{
			return SerializeSelections(selections2);
		}
		if (value is IEnumerable<ISelectItem> source)
		{
			return SerializeSelections(source.Select((ISelectItem x) => new KeyValuePair<string, object>(x.Text, x.Value)));
		}
		string text = JsonSerializer.Serialize(value);
		try
		{
			JsonSerializer.Deserialize<EditorDefinitionSelections>(text);
			return text;
		}
		catch (Exception innerException)
		{
			throw new NotSupportedException("Invalid format of selections", innerException);
		}
	}

	internal static EditorDefinitionDDSO SerializeSettingSelections(this EditorDefinitionDDSO editorDefinition)
	{
		if (editorDefinition.Settings != null && editorDefinition.Settings.TryGetValue("selections", out var value))
		{
			editorDefinition.Settings["selections"] = SerializeSelections(value);
		}
		return editorDefinition;
	}

	internal static EditorDefinitionDDSO DeserializeSettingSelections(this EditorDefinitionDDSO editorDefinition)
	{
		if (editorDefinition.Settings != null && editorDefinition.Settings.TryGetValue("selections", out var value) && value is string json)
		{
			try
			{
				editorDefinition.Settings["selections"] = JsonSerializer.Deserialize<EditorDefinitionSelections>(json);
			}
			catch (Exception)
			{
			}
		}
		return editorDefinition;
	}
}
