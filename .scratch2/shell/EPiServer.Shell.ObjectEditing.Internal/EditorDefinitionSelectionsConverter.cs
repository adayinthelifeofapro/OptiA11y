using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPiServer.Shell.ObjectEditing.Internal;

internal class EditorDefinitionSelectionsConverter : JsonConverter<EditorDefinitionSelections>
{
	public override EditorDefinitionSelections Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.Null)
		{
			return null;
		}
		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new InvalidOperationException();
		}
		EditorDefinitionSelections editorDefinitionSelections = new EditorDefinitionSelections();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
		{
			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				throw new JsonException();
			}
			string key = reader.GetString();
			reader.Read();
			object obj = JsonSerializer.Deserialize<object>(ref reader, options);
			obj = UnwrapJsonElement(obj);
			editorDefinitionSelections.Add(new KeyValuePair<string, object>(key, obj));
		}
		if (reader.TokenType != JsonTokenType.EndObject)
		{
			throw new InvalidOperationException();
		}
		return editorDefinitionSelections;
	}

	public override void Write(Utf8JsonWriter writer, EditorDefinitionSelections value, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		foreach (KeyValuePair<string, object> item in value)
		{
			writer.WritePropertyName(item.Key);
			JsonSerializer.Serialize(writer, item.Value, options);
		}
		writer.WriteEndObject();
	}

	private static object UnwrapJsonElement(object obj)
	{
		if (obj is JsonElement jsonElement)
		{
			switch (jsonElement.ValueKind)
			{
			case JsonValueKind.String:
				return jsonElement.GetString();
			case JsonValueKind.Number:
			{
				if (jsonElement.TryGetInt64(out var value))
				{
					return value;
				}
				return jsonElement.GetDouble();
			}
			case JsonValueKind.True:
			case JsonValueKind.False:
				return jsonElement.GetBoolean();
			case JsonValueKind.Null:
				return null;
			case JsonValueKind.Array:
			{
				List<object> list = new List<object>();
				{
					foreach (JsonElement item in jsonElement.EnumerateArray())
					{
						list.Add(UnwrapJsonElement(item));
					}
					return list;
				}
			}
			case JsonValueKind.Object:
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				{
					foreach (JsonProperty item2 in jsonElement.EnumerateObject())
					{
						dictionary[item2.Name] = UnwrapJsonElement(item2.Value);
					}
					return dictionary;
				}
			}
			default:
				return jsonElement.ToString();
			}
		}
		return obj;
	}
}
