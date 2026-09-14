using System;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPiServer.Shell.Json;

/// <summary>
///       Base class for converters that needs to create <see cref="T:System.Text.Json.JsonSerializerOptions" /> without the converter itself
///       </summary>
/// <typeparam name="T">The type to convert</typeparam>
public abstract class CachedOptionsJsonConverter<T> : JsonConverter<T>
{
	internal const int MaxCache = 100;

	internal ConcurrentDictionary<JsonSerializerOptions, JsonSerializerOptions> _optionsCache = new ConcurrentDictionary<JsonSerializerOptions, JsonSerializerOptions>(Environment.ProcessorCount, 100);

	/// <summary>
	///       Gets or creates an <see cref="T:System.Text.Json.JsonSerializerOptions" /> without the converter it self.
	///       </summary>
	/// <param name="originalOptions">The original option instance.</param>
	/// <returns>An option instance without the converter it self.</returns>
	protected internal JsonSerializerOptions GetOptionsWithoutMe(JsonSerializerOptions originalOptions)
	{
		return GetOptionsWithoutConverter(originalOptions, this);
	}

	/// <summary>
	///       Gets or creates an <see cref="T:System.Text.Json.JsonSerializerOptions" /> without the converter it self.
	///       </summary>
	/// <param name="originalOptions">The original option instance.</param>
	/// <param name="jsonConverter">The converter to remove from options</param>
	/// <returns>An option instance without the converter it self.</returns>
	protected internal JsonSerializerOptions GetOptionsWithoutConverter(JsonSerializerOptions originalOptions, JsonConverter jsonConverter)
	{
		if (_optionsCache.TryGetValue(originalOptions, out var value))
		{
			return value;
		}
		if (_optionsCache.Count >= 100)
		{
			_optionsCache.Clear();
		}
		JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions(originalOptions);
		jsonSerializerOptions.Converters.Remove(jsonConverter);
		return _optionsCache.GetOrAdd(originalOptions, jsonSerializerOptions);
	}
}
