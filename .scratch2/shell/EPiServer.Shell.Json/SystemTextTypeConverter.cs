using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPiServer.Shell.Json;

internal class SystemTextTypeConverter : JsonConverter<Type>, IJsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Type);
	}

	public override Type Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new InvalidOperationException("Cannot resolve types from type full name. Use a string instead if you need to resolve a type from a client object.");
	}

	public override void Write(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.FullName.ToLowerInvariant());
	}
}
