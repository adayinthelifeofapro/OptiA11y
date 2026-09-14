using System;
using System.Collections.Concurrent;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;

namespace EPiServer.Shell;

public static class ShellModuleStaticFilesRegistration
{
	private static readonly ConcurrentDictionary<string, string> _modulesPath = new ConcurrentDictionary<string, string>();

	public static void RegisterStaticClientResourceCaching(this IServiceCollection services, string moduleName)
	{
		services.Configure(moduleName, delegate(StaticFileOptions options)
		{
			options.OnPrepareResponse = delegate(StaticFileResponseContext fileContext)
			{
				string orAdd = _modulesPath.GetOrAdd(moduleName, (string key) => ServiceProviderExtensions.GetInstance<ModuleTable>(fileContext.Context.RequestServices).FindModule(key).ClientResourcePath.ToLower());
				if (fileContext.Context.Request.Path.Value.StartsWith(orAdd, StringComparison.OrdinalIgnoreCase) && !fileContext.Context.Response.Headers.ContainsKey(HeaderNames.CacheControl))
				{
					fileContext.Context.Response.Headers.Append(HeaderNames.CacheControl, CacheControlHeaderValue.PrivateString + ", " + CacheControlHeaderValue.MaxAgeString + "=31536000");
				}
			};
		});
	}
}
