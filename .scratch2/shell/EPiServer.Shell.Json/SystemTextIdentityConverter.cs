using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using EPiServer.Data;

namespace EPiServer.Shell.Json;

internal class SystemTextIdentityConverter : JsonConverter<Identity>, IJsonConverter
{
	private const char _escapeChar = '_';

	public override bool CanConvert(Type objectType)
	{
		return typeof(Identity).IsAssignableFrom(objectType);
	}

	public override Identity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string text = reader.GetString();
		if (!string.IsNullOrEmpty(text))
		{
			return Identity.Parse(text.Replace('_', ':'));
		}
		return null;
	}

	public override void Write(Utf8JsonWriter writer, Identity value, JsonSerializerOptions options)
	{
		ArgumentNullException.ThrowIfNull(writer, "writer");
		ArgumentNullException.ThrowIfNull(value, "value");
		writer.WriteStringValue(((object)value).ToString().Replace(':', '_'));
	}
}
