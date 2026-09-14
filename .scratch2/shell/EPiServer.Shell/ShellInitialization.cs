using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Localization.XmlResources;
using EPiServer.Licensing.Services;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Configuration;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell;

/// <summary>
///       Shell Initialization module
///       </summary>
[InitializableModule]
[ModuleDependency(typeof(FrameworkInitialization))]
public class ShellInitialization : IInitializableModule, IDisposable
{
	private const string ProductModulesProviderName = "EPiServerShellProductModulesResources";

	private const string ModulesProviderName = "EPiServerShellModulesResources";

	private bool _disposed;

	private Task _initializeModulesTask;

	private IServiceProvider _locator;

	private static IEnumerable<ShellModule> GetConfiguredModules(IServiceProvider locator)
	{
		return ShellModule.MergeDuplicateModules(locator.GetServices<IModuleProvider>().SelectMany((IModuleProvider p) => p.GetModules()));
	}

	/// <summary>
	///       Initializes this instance.
	///       </summary>
	/// <param name="context">The context.</param>
	public void Initialize(InitializationEngine context)
	{
		ILogger<ShellInitialization> logger = ServiceProviderExtensions.GetInstance<ILogger<ShellInitialization>>(context.Services);
		Task.Run(() => new XmlSerializer(typeof(ShellModuleManifest)));
		_locator = context.Services;
		logger.InitializingShellModules();
		List<ShellModule> list = GetConfiguredModules(_locator).ToList();
		ModuleTable service = _locator.GetService<ModuleTable>();
		service.AddRange(list);
		Assembly[] shellModuleAssemblies = list.Where((ShellModule m) => m.Name != "App").SelectMany((ShellModule m) => m.Assemblies).ToArray();
		ApplicationPartManager requiredService = _locator.GetRequiredService<ApplicationPartManager>();
		AssemblyPart[] array = (from a in requiredService.ApplicationParts.OfType<AssemblyPart>()
			where shellModuleAssemblies.Contains(a.Assembly, null)
			select a).ToArray();
		foreach (AssemblyPart item in array)
		{
			requiredService.ApplicationParts.Remove(item);
		}
		Task[] localizationTasks = InitializeLocalization(_locator, service.GetModules());
		context.InitComplete += delegate
		{
			WaitForInitializeModules(logger);
			Task.WaitAll(localizationTasks, CancellationToken.None);
		};
		LicensingServices.Instance.RegisterService<ILicensedExtendedDataService>((ILicensedExtendedDataService)(object)new LicensedExtendedDataService(ServiceLocator.Current));
		logger.ShellModuleInitializationComplete();
	}

	private void WaitForInitializeModules(ILogger<ShellInitialization> logger)
	{
		try
		{
			_initializeModulesTask?.Wait();
		}
		catch (AggregateException ex)
		{
			logger.LogInitializationError(ex);
			throw ex.InnerException;
		}
	}

	private Task[] InitializeLocalization(IServiceProvider locator, IEnumerable<ShellModule> modules)
	{
		List<Assembly> source = ServiceProviderExtensions.GetInstance<ModuleSorter>(_locator).GetSortedShellModules(modules).SelectMany((ShellModule m) => m.Assemblies)
			.ToList();
		AssemblySorter instance = ServiceProviderExtensions.GetInstance<AssemblySorter>(_locator);
		Assembly[] productModuleAssemblies = instance.GetSortedAssemblies(source.Where((Assembly a) => AssemblyExtension.IsEPiServerSignedAssembly(a))).ToArray();
		Assembly[] otherModuleAssemblies = source.Where((Assembly a) => !AssemblyExtension.IsEPiServerSignedAssembly(a)).ToArray();
		Task task = Task.Run(delegate
		{
			InitializeShellModuleLocalizationProviders(locator, otherModuleAssemblies, "EPiServerShellModulesResources");
		});
		Task task2 = Task.Run(delegate
		{
			InitializeShellModuleLocalizationProviders(locator, productModuleAssemblies, "EPiServerShellProductModulesResources");
		});
		return new Task[2] { task, task2 };
	}

	private void InitializeShellModuleLocalizationProviders(IServiceProvider locator, Assembly[] assemblies, string providerName)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		LocalizationService? service = locator.GetService<LocalizationService>();
		ProviderBasedLocalizationService val = (ProviderBasedLocalizationService)(object)((service is ProviderBasedLocalizationService) ? service : null);
		if (val != null)
		{
			XmlLocalizationProvider initializedProvider = new EmbeddedXmlLocalizationProviderInitializer().GetInitializedProvider(providerName, assemblies);
			val.AddProvider((LocalizationProvider)(object)initializedProvider);
		}
	}

	/// <summary>
	///       Resets the module into an uninitialized state.
	///       </summary>
	/// <param name="context">The context.</param>
	public void Uninitialize(InitializationEngine context)
	{
		UninitializeLocalizationProvider(context);
	}

	private static void UninitializeLocalizationProvider(InitializationEngine context)
	{
		LocalizationService instance = ServiceProviderExtensions.GetInstance<LocalizationService>(context.Services);
		ProviderBasedLocalizationService val = (ProviderBasedLocalizationService)(object)((instance is ProviderBasedLocalizationService) ? instance : null);
		if (val == null)
		{
			return;
		}
		foreach (LocalizationProvider item in val.ProviderList.Where((LocalizationProvider p) => p.Name.Equals("EPiServerShellProductModulesResources", StringComparison.Ordinal) || p.Name.Equals("EPiServerShellModulesResources", StringComparison.Ordinal)).ToList())
		{
			val.RemoveProvider(item.Name);
		}
	}

	/// <summary>
	///       Releases unmanaged and - optionally - managed resources
	///       </summary>
	/// <param name="disposing">
	///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_initializeModulesTask?.Dispose();
				_initializeModulesTask = null;
			}
			_disposed = true;
		}
	}

	/// <summary>
	///       Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
	///       </summary>
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
