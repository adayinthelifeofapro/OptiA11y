using System;
using System.Linq;
using System.Reflection;
using EPiServer.Shell.Routing.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.Routing;

public static class CustomShellModuleRouteRegistrationExtensions
{
	public static void MapCustomShellModuleRoutes(this IEndpointRouteBuilder endpointRouteBuilder, Assembly moduleAssembly, Action<IEndpointRouteBuilder> registration)
	{
		ApplicationPartManager requiredService = endpointRouteBuilder.ServiceProvider.GetRequiredService<ApplicationPartManager>();
		AssemblyPart assemblyPart = requiredService.ApplicationParts.OfType<AssemblyPart>().FirstOrDefault((AssemblyPart a) => a.Assembly.Equals(moduleAssembly));
		bool num = assemblyPart != null;
		if (!num)
		{
			assemblyPart = new AssemblyPart(moduleAssembly);
			requiredService.ApplicationParts.Add(assemblyPart);
		}
		registration(endpointRouteBuilder);
		if (!num)
		{
			endpointRouteBuilder.ServiceProvider.GetRequiredService<NotifyActionDescriptorChanged>().SignalChanged();
			requiredService.ApplicationParts.Remove(assemblyPart);
		}
	}
}
