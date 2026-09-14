using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using EPiServer.Formatters;
using EPiServer.Shell.Serialization.Json.Internal;
using Microsoft.Extensions.Options;

namespace EPiServer.Shell.Json.Internal;

internal class DefaultSystemTextJsonSettingsOptionsConfigurer : IConfigureNamedOptions<SystemTextJsonSettingsOptions>, IConfigureOptions<SystemTextJsonSettingsOptions>
{
	private readonly IEnumerable<JsonConverter> _converters;

	public DefaultSystemTextJsonSettingsOptionsConfigurer(IEnumerable<IJsonConverter> converters)
	{
		_converters = converters.OfType<JsonConverter>();
	}

	public void Configure(SystemTextJsonSettingsOptions options)
	{
		DefaultConfiguration(options.SerializerOptions.JsonSerializerOptions);
	}

	public void Configure(string name, SystemTextJsonSettingsOptions options)
	{
		DefaultConfiguration(options.SerializerOptions.JsonSerializerOptions);
	}

	private void DefaultConfiguration(JsonSerializerOptions jsonSerializerOptions)
	{
		foreach (JsonConverter converter in _converters)
		{
			jsonSerializerOptions.Converters.Add(converter);
		}
		DefaultSystemTextJsonNamingPolicy dictionaryKeyPolicy = (DefaultSystemTextJsonNamingPolicy)(jsonSerializerOptions.PropertyNamingPolicy = new DefaultSystemTextJsonNamingPolicy());
		jsonSerializerOptions.DictionaryKeyPolicy = dictionaryKeyPolicy;
	}
}
