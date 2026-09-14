using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EPiServer.Formatters;
using EPiServer.Framework.Serialization;

namespace EPiServer.Shell.Serialization.Json.Internal;

/// <summary>Unsupported INTERNAL API! Not covered by semantic versioning; might change without notice.</summary>
/// <internal-api />
public class SystemTextJsonObjectSerializer(SystemTextJsonSettingsOptions settingOptions) : IObjectSerializer
{
	private static readonly IEnumerable<string> _handledContentTypes = new List<string> { "application/json" }.AsReadOnly();

	private readonly JsonSerializerOptions _jsonSerializerOptions = settingOptions.SerializerOptions.JsonSerializerOptions;

	public IEnumerable<string> HandledContentTypes => _handledContentTypes;

	public void Serialize(TextWriter textWriter, object value)
	{
		textWriter.Write(JsonSerializer.Serialize(value, value?.GetType(), _jsonSerializerOptions));
	}

	public object Deserialize(TextReader reader, Type objectType)
	{
		return JsonSerializer.Deserialize(reader.ReadToEnd(), objectType, _jsonSerializerOptions);
	}

	public T Deserialize<T>(TextReader reader)
	{
		return (T)Deserialize(reader, typeof(T));
	}
}
