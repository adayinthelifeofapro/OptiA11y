using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Framework.TypeScanner;
using EPiServer.Shell.Json.Internal;
using EPiServer.Shell.Modules;
using EPiServer.Shell.Routing.Internal;
using EPiServer.Shell.Services.Rest;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Matching;
using EPiServer.Web.Routing.Matching.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EPiServer.Shell.Routing;

internal class ShellEndpointRoutingExtension : IEndpointRoutingExtension, IIsolateEndpointRegistration
{
	private readonly IEnumerable<IModuleProvider> _moduleProviders;

	private readonly ModuleInitializer _moduleInitializer;

	private readonly ApplicationPartManager _applicationPartManager;

	private readonly ITypeScannerLookup _typeScannerLookup;

	private readonly NotifyActionDescriptorChanged _notifyActionDescriptorChanged;

	private readonly IOptionsMonitor<StaticFileOptions> _staticFileOptionsAccessor;

	public ShellEndpointRoutingExtension(IEnumerable<IModuleProvider> moduleProviders, ModuleInitializer moduleInitializer, ApplicationPartManager applicationPartManager, ITypeScannerLookup typeScannerLookup, NotifyActionDescriptorChanged notifyActionDescriptorChanged, IOptionsMonitor<StaticFileOptions> staticFileOptionsAccessor)
	{
		_moduleProviders = moduleProviders;
		_moduleInitializer = moduleInitializer;
		_applicationPartManager = applicationPartManager;
		_typeScannerLookup = typeScannerLookup;
		_notifyActionDescriptorChanged = notifyActionDescriptorChanged;
		_staticFileOptionsAccessor = staticFileOptionsAccessor;
	}

	public void MapEndpoints(IEndpointRouteBuilder endpointRouteBuilder)
	{
		IEnumerable<ShellModule> enumerable = ShellModule.MergeDuplicateModules(_moduleProviders.SelectMany((IModuleProvider p) => p.GetModules()).ToArray());
		AssemblyPart[] shellAssemblyPartList = RegisterShellAssemblies(enumerable);
		_notifyActionDescriptorChanged.SignalChanged();
		endpointRouteBuilder.ServiceProvider.GetRequiredService<ShellModuleMvcOptionsConfigurer>().Initialize(enumerable);
		endpointRouteBuilder.ServiceProvider.GetRequiredService<ShellModuleFormatterOptionsConfigurer>().Initialize(enumerable);
		InitializeModules(endpointRouteBuilder, enumerable);
		CollectEndpointsAndRestoreAsseblyParts(endpointRouteBuilder, shellAssemblyPartList);
	}

	private void InitializeModules(IEndpointRouteBuilder routeBuilder, IEnumerable<ShellModule> modules)
	{
		_moduleInitializer.RegisterModules(modules, routeBuilder);
		_moduleInitializer.RegisterClientResourceRoutes(modules, routeBuilder);
		_moduleInitializer.MapStaticFiles("Util", "util/{folder}/{**slug:epi-staticfile}", routeBuilder, _staticFileOptionsAccessor.Get("Util"));
		_moduleInitializer.MapStaticFiles("App_Themes", "app_themes/{folder}/{**slug:epi-staticfile}", routeBuilder, _staticFileOptionsAccessor.Get("App_Themes"));
		IEnumerable<Type> restStoreTypes = from t in _typeScannerLookup.AllTypes
			where typeof(RestControllerBase).IsAssignableFrom(t)
			where !t.IsAbstract
			where t.GetCustomAttributes(typeof(RestStoreAttribute), inherit: true).Length != 0
			select t;
		_moduleInitializer.RegisterRestStores(modules, restStoreTypes, routeBuilder);
		RegisterAccountEndpoints(routeBuilder);
		RegisterErrorEndpoints(routeBuilder);
	}

	private void RegisterAccountEndpoints(IEndpointRouteBuilder routeBuilder)
	{
		routeBuilder.MapControllerRoute("Login", "/Util/Login", new
		{
			controller = "Account",
			action = "Login"
		});
		routeBuilder.MapControllerRoute("ExecuteLogin", "/Util/Login", new
		{
			controller = "Account",
			action = "ExecuteLogin"
		}, new
		{
			httpMethod = new HttpMethodRouteConstraint(HttpMethods.Post)
		});
		routeBuilder.MapControllerRoute("Logout", "/Util/Logout", new
		{
			controller = "Account",
			action = "Logout"
		});
		routeBuilder.MapControllerRoute("RegisterAdminUser", "/Util/Register", new
		{
			controller = "RegisterAdminUser",
			action = "Index"
		});
	}

	private void RegisterErrorEndpoints(IEndpointRouteBuilder routeBuilder)
	{
		routeBuilder.MapControllerRoute("Error500", "/Util/Errors/Error500", new
		{
			controller = "Errors",
			action = "Error500"
		});
		routeBuilder.MapControllerRoute("Error404", "/Util/Errors/Error404", new
		{
			controller = "Errors",
			action = "Error404"
		});
		routeBuilder.MapControllerRoute("Error403", "/Util/Errors/Error403", new
		{
			controller = "Errors",
			action = "Error403"
		});
	}

	private AssemblyPart[] RegisterShellAssemblies(IEnumerable<ShellModule> modules)
	{
		AssemblyPart[] currentParts = _applicationPartManager.ApplicationParts.OfType<AssemblyPart>().ToArray();
		AssemblyPart[] array = (from a in modules.SelectMany((ShellModule m) => m.Assemblies)
			where !currentParts.Any((AssemblyPart cp) => cp.Assembly.Equals(a))
			select new AssemblyPart(a)).ToArray();
		AssemblyPart[] array2 = array;
		foreach (AssemblyPart item in array2)
		{
			_applicationPartManager.ApplicationParts.Add(item);
		}
		return array;
	}

	private void CollectEndpointsAndRestoreAsseblyParts(IEndpointRouteBuilder routeBuilder, AssemblyPart[] shellAssemblyPartList)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		List<Endpoint> list = routeBuilder.DataSources.SelectMany((EndpointDataSource e) => e.Endpoints).ToList();
		Endpoint[] array = list.Where((Endpoint e) => IsNoneShellAttributeRoute(e, shellAssemblyPartList)).ToArray();
		foreach (Endpoint item in array)
		{
			list.Remove(item);
		}
		ImmutableEndpointDataSource val = new ImmutableEndpointDataSource(list.ToArray());
		routeBuilder.DataSources.Clear();
		routeBuilder.DataSources.Add((EndpointDataSource)(object)val);
		routeBuilder.ServiceProvider.GetServices<MatcherPolicy>().OfType<MethodOverrideMatcherPolicy>().Single()
			.SetShellEndpoints((EndpointDataSource)(object)val);
		AssemblyPart[] array2 = shellAssemblyPartList;
		foreach (AssemblyPart item2 in array2)
		{
			_applicationPartManager.ApplicationParts.Remove(item2);
		}
		_notifyActionDescriptorChanged.SignalChanged();
	}

	private bool IsNoneShellAttributeRoute(Endpoint endpoint, AssemblyPart[] shellAssemblies)
	{
		if (endpoint is RouteEndpoint)
		{
			ControllerActionDescriptor controllerDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
			if (controllerDescriptor != null && controllerDescriptor.AttributeRouteInfo != null)
			{
				return !shellAssemblies.Any((AssemblyPart a) => a.Assembly == controllerDescriptor.ControllerTypeInfo.Assembly);
			}
			return false;
		}
		return false;
	}
}
