using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Serialization.Json.Internal;

[ServiceConfiguration(typeof(JsonConverter))]
public class DynamicConverter : JsonConverter<object>
{
	private static readonly ConcurrentDictionary<Type, IList<JsonConverter>> _typeConverters = new ConcurrentDictionary<Type, IList<JsonConverter>>();

	public void Register(Type type, IList<JsonConverter> converters)
	{
		_typeConverters.AddOrUpdate(type, converters, delegate(Type k, IList<JsonConverter> v)
		{
			foreach (JsonConverter converter in converters)
			{
				if (!v.Any((JsonConverter x) => x.GetType() == converter.GetType()))
				{
					v.Add(converter);
				}
			}
			return v;
		});
	}

	private static void ApplyTypeConverters(JsonSerializerOptions options, Type type)
	{
		DynamicConverter dynamicConverter = options.Converters.OfType<DynamicConverter>().FirstOrDefault();
		if (dynamicConverter != null)
		{
			options.Converters.Remove(dynamicConverter);
		}
		if (!_typeConverters.TryGetValue(type, out var value) || !value.Any())
		{
			return;
		}
		foreach (JsonConverter converter in value)
		{
			if (!options.Converters.Any((JsonConverter x) => x.GetType() == converter.GetType()))
			{
				options.Converters.Add(converter);
			}
		}
	}

	public override bool CanConvert(Type typeToConvert)
	{
		return _typeConverters.ContainsKey(typeToConvert);
	}

	public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		ApplyTypeConverters(options, typeToConvert);
		using JsonDocument jsonDocument = JsonDocument.ParseValue(ref reader);
		return JsonSerializer.Deserialize(jsonDocument.RootElement.GetRawText(), typeToConvert, options);
	}

	public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
	{
		ApplyTypeConverters(options, value.GetType());
		JsonSerializer.Serialize(writer, value, value.GetType(), options);
	}
}
