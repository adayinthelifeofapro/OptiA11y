using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using EPiServer.Framework;
using EPiServer.Framework.Hosting;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Web.Resources;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Handles convention based registration of <see cref="T:EPiServer.Web.Hosting.ZipArchiveFileProvider" /> for modules.
///       </summary>
/// <remarks>
///   <para>
///           If a module contains a .zip archive file with the same name as the directory it will be registered
///           and the content of the archive exposed through a <see cref="T:EPiServer.Web.Hosting.ZipArchiveFileProvider" />.
///       </para>
///   <para>
///           This class supports the EPiServer infrastructure and is not intended to be used directly from your code.
///       </para>
/// </remarks>
[InitializableModule]
public class ShellZipArchiveVirtualPathProviderModule : IInitializableModule, IFileProviderModule
{
	private ZipArchiveFinder _archiveFinder;

	private ILoggerFactory _loggerFactory;

	private ILogger<ShellZipArchiveVirtualPathProviderModule> _log;

	[CompilerGenerated]
	private string _003CPublicModulesRootPath_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CProtectedModulesRootPath_003Ek__BackingField;

	internal Func<string, string, IBasePathFileProvider> ProviderFactoryMethod { get; set; }

	internal string PublicModulesRootPath
	{
		get
		{
			return _003CPublicModulesRootPath_003Ek__BackingField ?? Paths.PublicRootPath;
		}
		[CompilerGenerated]
		set
		{
			_003CPublicModulesRootPath_003Ek__BackingField = value;
		}
	}

	internal string ProtectedModulesRootPath
	{
		get
		{
			return _003CProtectedModulesRootPath_003Ek__BackingField ?? Paths.ProtectedRootPath;
		}
		[CompilerGenerated]
		set
		{
			_003CProtectedModulesRootPath_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ShellZipArchiveVirtualPathProviderModule" /> class.
	///       </summary>
	public ShellZipArchiveVirtualPathProviderModule()
	{
		ProviderFactoryMethod = delegate(string virtualPath, string archivePath)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			return (IBasePathFileProvider)new ZipArchiveFileProvider(virtualPath, archivePath);
		};
	}

	internal ShellZipArchiveVirtualPathProviderModule(ZipArchiveFinder zipArchiveFinder, ILogger<ShellZipArchiveVirtualPathProviderModule> log, ILoggerFactory loggerFactory)
		: this()
	{
		_archiveFinder = zipArchiveFinder;
		_log = log;
		_loggerFactory = loggerFactory;
		ProviderFactoryMethod = delegate(string virtualPath, string archivePath)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Expected O, but got Unknown
			return (IBasePathFileProvider)new ZipArchiveFileProvider(virtualPath, archivePath);
		};
	}

	/// <inheritdoc />
	public IEnumerable<IBasePathFileProvider> CreateProviders(InitializationEngine context)
	{
		if ((int)context.HostType != 1 || string.IsNullOrEmpty(PublicModulesRootPath))
		{
			yield break;
		}
		ClientResourceOptions clientResourceOptions = ServiceProviderExtensions.GetInstance<ClientResourceOptions>(context.Services);
		foreach (IBasePathFileProvider item in CreateProviders(PublicModulesRootPath, PublicModulesRootPath, clientResourceOptions))
		{
			yield return item;
		}
		string internalRootPath = VirtualPathUtilityEx.AppendTrailingSlash(UriUtil.Combine(PublicModulesRootPath, "_protected"));
		foreach (IBasePathFileProvider item2 in CreateProviders(internalRootPath, ProtectedModulesRootPath, clientResourceOptions))
		{
			yield return item2;
		}
	}

	private IEnumerable<IBasePathFileProvider> CreateProviders(string internalRootPath, string externalRootPath, ClientResourceOptions clientResourceOptions)
	{
		IDictionary<string, string> dictionary = _archiveFinder.Find(internalRootPath);
		if (dictionary == null || dictionary.Count == 0)
		{
			yield break;
		}
		foreach (KeyValuePair<string, string> item in dictionary)
		{
			_log.RegisteringZipArchiveVirtualPathProvider(item.Key);
			string arg = VirtualPathUtilityEx.AppendTrailingSlash(UriUtil.Combine(externalRootPath, item.Key));
			string arg2 = Path.Combine(internalRootPath, item.Value);
			IBasePathFileProvider fileProvider = ProviderFactoryMethod(arg, arg2);
			if (clientResourceOptions.Debug)
			{
				yield return (IBasePathFileProvider)(object)new DebugZipArchiveFileProviderDecorator(fileProvider, _loggerFactory.CreateLogger<DebugZipArchiveFileProviderDecorator>());
			}
			else
			{
				yield return (IBasePathFileProvider)(object)new ReleaseFileProviderDecorator(fileProvider);
			}
		}
	}

	public void Initialize(InitializationEngine context)
	{
		if (_archiveFinder == null)
		{
			_archiveFinder = ServiceProviderExtensions.GetInstance<ZipArchiveFinder>(context.Services);
		}
		_loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
		_log = context.Services.GetRequiredService<ILogger<ShellZipArchiveVirtualPathProviderModule>>();
		IEnumerable<IFileProviderModule> allInstances = ServiceProviderExtensions.GetAllInstances<IFileProviderModule>(context.Services);
		IEnumerable<IBasePathFileProvider> enumerable = CreateProviders(context);
		foreach (IFileProviderModule item in allInstances)
		{
			enumerable = enumerable.Union(item.CreateProviders(context));
		}
		if (enumerable.Any())
		{
			ServiceProviderExtensions.GetInstance<ICompositeFileProvider>(context.Services).AddProviders(enumerable);
		}
	}

	public void Uninitialize(InitializationEngine context)
	{
	}
}
