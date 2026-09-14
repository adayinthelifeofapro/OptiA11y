using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using EPiServer.Licensing.Services;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell;

/// <summary>
///       Integrates the modules concept into the licensing system
///       </summary>
internal class LicensedExtendedDataService : ILicensedExtendedDataService
{
	private static readonly Lock _syncObj = new Lock();

	private readonly IServiceProvider _serviceProvider;

	private ExtendedData _extendedData;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.LicensedExtendedDataService" /> class.
	///       <param name="serviceProvider"></param></summary>
	public LicensedExtendedDataService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider ?? ServiceLocator.Current;
	}

	/// <summary>
	///       Get licensed extended data such as modules name
	///       </summary>
	/// <returns>
	/// </returns>
	public ExtendedData Get()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (_extendedData == null)
		{
			using (_syncObj.EnterScope())
			{
				if (_extendedData == null)
				{
					ExtendedData val = new ExtendedData();
					JsonArray jsonArray = new JsonArray();
					foreach (JsonObject module in GetModules())
					{
						jsonArray.Add(module);
					}
					val.Data.Add("modules", jsonArray);
					_extendedData = val;
				}
			}
		}
		return _extendedData;
	}

	private IEnumerable<JsonObject> GetModules()
	{
		return ShellModule.MergeDuplicateModules(_serviceProvider.GetServices<IModuleProvider>().SelectMany((IModuleProvider p) => p.GetModules())).Select(ConvertToJsonObject);
	}

	private JsonObject ConvertToJsonObject(ShellModule sm)
	{
		return new JsonObject
		{
			{ "name", sm.Name },
			{
				"version",
				sm.ResolveVersion().ToString()
			}
		};
	}
}
