using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPiServer.Shell.Json;

/// <summary>
///       Writes the FullName property for a <see cref="T:System.Type" /> instead of the FullyQualifiedName.
///       </summary>
internal class SystemTextTypeArrayConverter : JsonConverter<Type[]>, IJsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(Type[]);
	}

	public override Type[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		throw new InvalidOperationException("Cannot resolve types from type full name. Use a string instead if you need to resolve a type from a client object.");
	}

	public override void Write(Utf8JsonWriter writer, Type[] value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		for (int i = 0; i < value.Length; i++)
		{
			writer.WriteStringValue(value[i].FullName.ToLowerInvariant());
		}
		writer.WriteEndArray();
	}
}
