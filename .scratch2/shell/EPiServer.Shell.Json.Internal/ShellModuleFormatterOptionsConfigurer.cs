using System.Collections.Generic;
using System.Reflection;
using EPiServer.Formatters;
using EPiServer.Shell.Modules;
using EPiServer.Shell.Serialization.Json.Internal;

namespace EPiServer.Shell.Json.Internal;

internal class ShellModuleFormatterOptionsConfigurer(JsonFormatterOptions options, CompositeJsonObjectSerializer compositeJsonObjectSerializer, SystemTextJsonObjectSerializer systemTextJsonObjectSerializer)
{
	/// <summary>
	///       Allows initialization with a set of modules
	///       </summary>
	/// <param name="uniqueModules">
	/// </param>
	public void Initialize(IEnumerable<ShellModule> uniqueModules)
	{
		foreach (ShellModule uniqueModule in uniqueModules)
		{
			RegisterSystemTextJsonForAssemblies(uniqueModule);
		}
	}

	private void RegisterSystemTextJsonForAssemblies(ShellModule module)
	{
		foreach (Assembly assembly in module.Assemblies)
		{
			RegisterSystemTextInputFormatterIfNotAlreadyExist(assembly);
			RegisterSystemTextOutputFormatterIfNotAlreadyExist(assembly);
			compositeJsonObjectSerializer.Register(assembly, systemTextJsonObjectSerializer);
		}
	}

	private void RegisterSystemTextInputFormatterIfNotAlreadyExist(Assembly assembly)
	{
		if (!options.AssemblyModuleInputFormatters.ContainsKey(assembly))
		{
			JsonFormatterOptionsExtensions.UseSystemTextJsonInputFormatter(options, assembly, (string)null);
		}
	}

	private void RegisterSystemTextOutputFormatterIfNotAlreadyExist(Assembly assembly)
	{
		if (!options.AssemblyModuleOutputFormatters.ContainsKey(assembly))
		{
			JsonFormatterOptionsExtensions.UseSystemTextJsonOutputFormatter(options, assembly, (string)null);
		}
	}
}
