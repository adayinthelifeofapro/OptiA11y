using System;
using System.Collections.Generic;
using System.Text.Json;
using EPiServer.Shell.ViewComposition;

namespace EPiServer.Shell.Json;

internal class SystemTextSettingsDictionaryConverter : CachedOptionsJsonConverter<ISettingsDictionary>, IJsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return typeof(ISettingsDictionary).IsAssignableFrom(objectType);
	}

	public override ISettingsDictionary Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string text = null;
		SettingsDictionary settingsDictionary = new SettingsDictionary();
		while (reader.Read())
		{
			switch (reader.TokenType)
			{
			case JsonTokenType.PropertyName:
				text = reader.GetString();
				continue;
			case JsonTokenType.EndObject:
				return settingsDictionary;
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				settingsDictionary.Add(text, GetSettingValue(ref reader, options));
			}
		}
		return settingsDictionary;
	}

	public override void Write(Utf8JsonWriter writer, ISettingsDictionary value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value, GetOptionsWithoutMe(options));
	}

	private object GetSettingValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
		case JsonTokenType.String:
		{
			if (reader.TryGetDateTime(out var value))
			{
				return value;
			}
			return reader.GetString();
		}
		case JsonTokenType.False:
			return false;
		case JsonTokenType.True:
			return true;
		case JsonTokenType.Number:
		{
			if (reader.TryGetInt32(out var value2))
			{
				return value2;
			}
			return reader.GetDouble();
		}
		case JsonTokenType.StartObject:
			return Read(ref reader, null, options);
		case JsonTokenType.StartArray:
		{
			List<object> list = new List<object>();
			while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
			{
				list.Add(GetSettingValue(ref reader, options));
			}
			return list;
		}
		default:
			return null;
		}
	}
}
