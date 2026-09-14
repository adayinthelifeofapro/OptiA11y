using System.Text.Json;

namespace EPiServer.Shell.Serialization.Json.Internal;

internal class DefaultSystemTextJsonNamingPolicy : JsonNamingPolicy
{
	public override string ConvertName(string name)
	{
		return StringUtility.ToCamelCase(name);
	}
}
