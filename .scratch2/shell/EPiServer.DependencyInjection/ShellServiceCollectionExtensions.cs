using System;
using System.ComponentModel;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.Formatters;
using EPiServer.Framework.Modules;
using EPiServer.Framework.Serialization;
using EPiServer.Framework.Serialization.Internal;
using EPiServer.Framework.Web.Resources;
using EPiServer.Security;
using EPiServer.Shell;
using EPiServer.Shell.Internal;
using EPiServer.Shell.Json;
using EPiServer.Shell.Json.Internal;
using EPiServer.Shell.Licensing;
using EPiServer.Shell.Modules;
using EPiServer.Shell.Modules.Internal;
using EPiServer.Shell.Navigation;
using EPiServer.Shell.Navigation.Internal;
using EPiServer.Shell.Navigation.Providers;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.Internal;
using EPiServer.Shell.Profile;
using EPiServer.Shell.Profile.Internal;
using EPiServer.Shell.Routing;
using EPiServer.Shell.Routing.Internal;
using EPiServer.Shell.Search;
using EPiServer.Shell.Security.Internal;
using EPiServer.Shell.Serialization.Json.Internal;
using EPiServer.Shell.Services.Rest;
using EPiServer.Shell.UserMembership.Internal;
using EPiServer.Shell.ViewComposition;
using EPiServer.Shell.Web.Internal;
using EPiServer.Shell.Web.Resources;
using EPiServer.Web;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Internal;
using EPiServer.Web.Routing.Matching.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace EPiServer.DependencyInjection;

/// <summary>
///       Extension methods to <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to configure CMS Shell.
///       </summary>
public static class ShellServiceCollectionExtensions
{
	/// <summary>
	///       Adds CMS Shell services to the service collection.
	///       </summary>
	/// <param name="services">The services being configured.</param>
	/// <returns>The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to enable further configuration.</returns>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static IServiceCollection AddCmsShell(this IServiceCollection services)
	{
		ArgumentNullException.ThrowIfNull(services, "services");
		services.TryAddSingleton((Func<IServiceProvider, Func<IVirtualRoleAuthorizationSession>>)((IServiceProvider s) => () => s.GetRequiredService<IVirtualRoleAuthorizationSession>()));
		RoutingServiceCollectionExtensions.AddEndpointRoutingExtension<ShellEndpointRoutingExtension>(services);
		services.TryAddSingleton<NotifyActionDescriptorChanged>();
		ServiceCollectionExtensions.TryForwardEnumerable<NotifyActionDescriptorChanged, IActionDescriptorChangeProvider>(services);
		services.TryAddSingleton<ModuleTable>();
		services.TryAddTransient<ModuleInitializer>();
		services.TryAddEnumerable(ServiceDescriptor.Singleton<MatcherPolicy, MethodOverrideMatcherPolicy>());
		services.TryAddTransient<IPersonalizedViewSettingsRepository, PersonalizedViewSettingsRepository>();
		services.TryAddTransient<IModuleFinder, ModuleFinder>();
		services.TryAddEnumerable(ServiceDescriptor.Transient<IMenuProvider, ReflectingMenuItemProvider>());
		services.TryAddEnumerable(ServiceDescriptor.Transient<IMenuProvider, ModuleHelpMenuProvider>());
		services.TryAddEnumerable(ServiceDescriptor.Transient<IMenuProvider, ConfigNavigationProvider>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IClientResourceProvider, ModuleClientResourceProvider>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<ISearchProviderSource, IOCRegisteredSearchProviderSource>());
		services.TryAddSingleton<SearchProviderRegistry>();
		ServiceCollectionExtensions.TryForwardEnumerable<SearchProviderRegistry, ISearchProviderSource>(services);
		services.TryAddSingleton<ISearchProviderManager, SearchProviderManager>();
		services.TryAddEnumerable(ServiceDescriptor.Transient<ISearchProvider, NavigationSearchProvider>());
		services.TryAddSingleton<DefaultAccessReadOnlyProtectedModulesNotifier>();
		ServiceCollectionExtensions.TryForward<DefaultAccessReadOnlyProtectedModulesNotifier, IAccessReadOnlyProtectedModules>(services);
		ServiceCollectionExtensions.TryForward<DefaultAccessReadOnlyProtectedModulesNotifier, IAccessReadOnlyProtectedModulesNotifier>(services);
		services.TryAddSingleton((Func<IServiceProvider, IComponentManager>)((IServiceProvider s) => new ComponentManager(s.GetServices<IComponentProvider>(), s.GetService<SecuredComponentOptions>())));
		ServiceCollectionExtensions.AddServiceAccessor<IComponentManager>(services);
		services.TryAddEnumerable(ServiceDescriptor.Transient<IViewTransformer, ConfigurationViewTransformer>((IServiceProvider s) => new ConfigurationViewTransformer(s.GetService<ViewOptions>(), s.GetRequiredService<IComponentManager>())));
		ServiceCollectionExtensions.AddServiceAccessor<IViewTransformer>(services);
		services.TryAddTransient((IServiceProvider s) => s.GetService<IOptions<MvcViewOptions>>().Value.ClientModelValidatorProviders.FirstOrDefault());
		services.TryAddEnumerable(ServiceDescriptor.Scoped<IAuthorizationHandler, ReadOnlyProtectedModulesAuthorizationHandler>());
		services.TryAddTransient<ReadOnlyProtectedModulesValidator>();
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<AuthorizationOptions>, AuthorizationOptionsConfigurer>());
		services.ConfigureOptions<LicensingOptionsConfigurer>();
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IJsonConverter, SystemTextIdentityConverter>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IJsonConverter, SystemTextSettingsDictionaryConverter>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IJsonConverter, SystemTextTypeConverter>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IJsonConverter, SystemTextTypeArrayConverter>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IJsonConverter, SystemTextJsonModuleViewModelConverter>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<SystemTextJsonSettingsOptions>, DefaultSystemTextJsonSettingsOptionsConfigurer>());
		services.TryAddSingleton<SystemTextJsonObjectSerializer>();
		services.TryAddSingleton((IServiceProvider s) => new CompositeJsonObjectSerializer(s.GetRequiredService<SystemTextJsonObjectSerializer>()));
		ServiceCollectionExtensions.TryForward<CompositeJsonObjectSerializer, IObjectSerializer>(services);
		ServiceCollectionExtensions.AddServiceAccessor<IObjectSerializer>(services);
		services.TryAddSingleton<EditUrlResolver, DefaultEditUrlResolver>();
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IResolveType, AssemblyForwarding>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IApplicationModelProvider, AreaApplicationModelProvider>());
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<StaticFileOptions>, AppThemesAndUtilsOptionsConfigurer>());
		services.RegisterStaticClientResourceCaching("Shell");
		services.TryAddSingleton<AssemblySorter>();
		services.TryAddSingleton<ModuleSorter>();
		services.TryAddSingleton<IViewManager, DefaultViewManager>();
		services.TryAddSingleton<IEditorDefinitionRepository, EditorDefinitionRepository>();
		services.TryAddSingleton<ExtensibleMetadataProvider>();
		services.TryAddSingleton<IEPiResourcesService, EPiResourcesService>();
		services.TryAddSingleton<MetadataHandlerRegistry>();
		ServiceCollectionExtensions.AddServiceAccessor<MetadataHandlerRegistry>(services);
		services.TryAddSingleton<MenuAssembler>();
		services.TryAddScoped<NavigationService>();
		services.TryAddTransient<ICurrentUiCulture, NullCurrentUiCulture>();
		services.TryAddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
		ServiceCollectionExtensions.AddServiceAccessor<IObjectSerializerFactory>(services);
		services.TryAddTransient<PersonalizedViewSettingsManager>();
		services.TryAddSingleton<IProfileRepository, ProfileRepository>();
		services.TryAddTransient<ReadOnlyPageUrlResolver>();
		services.TryAddEnumerable(ServiceDescriptor.Transient<IRestControllerValueProvider, RestHeaderValuesProvider>());
		services.TryAddTransient<SecurityConfiguration>();
		ServiceCollectionExtensions.AddServiceAccessor<SecurityConfiguration>(services);
		services.Replace(ServiceDescriptor.Transient<IModuleResourceResolver, ShellModulePathResolver>());
		services.TryAddSingleton<UIDescriptorRegistry>();
		services.TryAddSingleton<UIPathResolver>();
		ServiceCollectionExtensions.AddServiceAccessor<UIPathResolver>(services);
		services.TryAddTransient<IUserMembershipService, UserMembershipService>();
		services.TryAddTransient<ZipArchiveFinder>();
		services.TryAddTransient<ShellModuleMvcOptionsConfigurer>();
		services.TryAddTransient<ShellModuleFormatterOptionsConfigurer>();
		return services;
	}
}
