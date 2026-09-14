using System;
using System.Text.Json;
using EPiServer.Shell.Modules;

namespace EPiServer.Shell.Json;

internal class SystemTextJsonModuleViewModelConverter : CachedOptionsJsonConverter<ModuleViewModel>, IJsonConverter
{
	public override ModuleViewModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return JsonSerializer.Deserialize<ModuleViewModel>(ref reader, GetOptionsWithoutMe(options));
	}

	public override void Write(Utf8JsonWriter writer, ModuleViewModel value, JsonSerializerOptions options)
	{
		JsonSerializer.Serialize(writer, value, value?.GetType(), GetOptionsWithoutMe(options));
	}
}
