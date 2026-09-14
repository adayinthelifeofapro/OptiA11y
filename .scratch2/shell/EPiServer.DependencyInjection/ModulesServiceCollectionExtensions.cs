using System;
using System.Linq;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.DependencyInjection;

/// <summary>
///       Extension methods to <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to configure CMS Shell modules.
///       </summary>
public static class ModulesServiceCollectionExtensions
{
	/// <summary>
	///       Adds a registration for a protected Shell module unless one is already registered.
	///       </summary>
	/// <param name="services">The services being configured.</param>
	/// <param name="moduleName">The name of the module that should be added.</param>
	/// <returns>The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to enable further configuration.</returns>
	public static IServiceCollection TryAddProtectedShellModule(this IServiceCollection services, string moduleName)
	{
		ArgumentNullException.ThrowIfNull(services, "services");
		ArgumentException.ThrowIfNullOrEmpty(moduleName, "moduleName");
		services.Configure(delegate(ProtectedModuleOptions o)
		{
			if (!o.Items.Any((ModuleDetails i) => i.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase)))
			{
				o.Items.Add(new ModuleDetails
				{
					Name = moduleName
				});
			}
		});
		return services;
	}
}
