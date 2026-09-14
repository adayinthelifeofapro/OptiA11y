using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Framework.Hosting;
using EPiServer.Framework.Initialization;
using EPiServer.Web.Hosting;
using EPiServer.Web.Hosting.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace EPiServer.Shell.Json.Internal;

internal class AppThemesAndUtilsOptionsConfigurer : IConfigureNamedOptions<StaticFileOptions>, IConfigureOptions<StaticFileOptions>
{
	private static readonly string[] _supportedOptionNames = new string[2] { "Util", "App_Themes" };

	private static readonly string[] _opeResources = new string[2] { "communicationInjector.js", "deliveryPropertyRenderer.js" };

	private readonly IWebHostEnvironment _webHostEnvironment;

	private readonly IBasePathFileProvider[] _basePathFileProviders;

	private readonly CompositeFileProvider _customCompositeFileProvider;

	public AppThemesAndUtilsOptionsConfigurer(IWebHostEnvironment webHostEnvironment, IOptions<CompositeFileProviderOptions> compositeFileProviderOptions, InitializationEngine initializationEngine, IEnumerable<IFileProviderModule> fileProviderModules)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		base._002Ector();
		_webHostEnvironment = webHostEnvironment;
		_basePathFileProviders = fileProviderModules.SelectMany((IFileProviderModule m) => m.CreateProviders(initializationEngine)).ToArray();
		_customCompositeFileProvider = new CompositeFileProvider((IEnumerable<IFileProvider>)new IFileProvider[1] { _webHostEnvironment.WebRootFileProvider }, compositeFileProviderOptions);
		_customCompositeFileProvider.AddProviders((IEnumerable<IBasePathFileProvider>)(from x in _basePathFileProviders
			where _supportedOptionNames.Any((string name) => x.BasePath.StartsWith("/" + name, StringComparison.OrdinalIgnoreCase))
			orderby x.BasePath.Length descending
			select x));
	}

	public void Configure(string name, StaticFileOptions options)
	{
		if (!_supportedOptionNames.Any((string x) => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
		{
			return;
		}
		options.FileProvider = (IFileProvider?)_customCompositeFileProvider;
		options.OnPrepareResponse = delegate(StaticFileResponseContext fileContext)
		{
			string requestPath = fileContext.Context.Request.Path.Value;
			if (_opeResources.Any((string s) => requestPath.Contains(s, StringComparison.OrdinalIgnoreCase)))
			{
				fileContext.Context.Response.Headers.Append(HeaderNames.CacheControl, CacheControlHeaderValue.MaxAgeString + "=900, " + CacheControlHeaderValue.MustRevalidateString);
			}
		};
	}

	public void Configure(StaticFileOptions options)
	{
	}
}
