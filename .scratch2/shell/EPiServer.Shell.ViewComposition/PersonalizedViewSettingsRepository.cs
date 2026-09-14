using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Shell.Storage;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Repository for PersonalizedViewSettings objects
///       </summary>
public class PersonalizedViewSettingsRepository : IPersonalizedViewSettingsRepository
{
	private readonly DynamicDataStoreFactory _ddsFactory;

	private readonly IComponentManager _componentManager;

	private readonly ILogger<PersonalizedViewSettingsRepository> _logger;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettingsRepository" /> class.
	///       </summary>
	/// <param name="dynamicDataStoreFactory">The dynamic data store factory.</param>
	/// <param name="componentManager">The component manager that is used to create components.</param>
	/// <param name="logger">The logger.</param>
	public PersonalizedViewSettingsRepository(DynamicDataStoreFactory dynamicDataStoreFactory, IComponentManager componentManager, ILogger<PersonalizedViewSettingsRepository> logger)
	{
		ArgumentNullException.ThrowIfNull(dynamicDataStoreFactory, "dynamicDataStoreFactory");
		_ddsFactory = dynamicDataStoreFactory;
		_componentManager = componentManager;
		_logger = logger;
	}

	/// <summary>
	///       Saves the specified settings.
	///       </summary>
	/// <param name="settings">The settings.</param>
	public void Save(PersonalizedViewSettings settings)
	{
		PersonalizedViewSettingsStorage personalizedViewSettingsStorage = PersonalizedViewSettingsStorage.CreateFromPersonalizedViewSettings(settings, _componentManager, _logger);
		GetStore().Save((object)personalizedViewSettingsStorage);
	}

	/// <summary>
	///       Deletes the settings object for the specified user and view thus restoring the view settings to system default.
	///       </summary>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="principal">The principal.</param>
	public void Delete(IPrincipal principal, string viewName)
	{
		PersonalizedViewSettingsStorage settings = GetSettings(principal, viewName);
		GetStore().Delete(Identity.op_Implicit(settings.Id));
	}

	/// <summary>
	///       Loads settings given specified view name.
	///       </summary>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="principal">The principal.</param>
	/// <returns>
	/// </returns>
	public PersonalizedViewSettings Load(IPrincipal principal, string viewName)
	{
		ArgumentNullException.ThrowIfNull(principal, "principal");
		if (string.IsNullOrEmpty(viewName))
		{
			throw new ArgumentException("Parameter must not be null or empty", "viewName");
		}
		return GetSettings(principal, viewName)?.ConvertToPersonalizedViewSettings(principal);
	}

	private PersonalizedViewSettingsStorage GetSettings(IPrincipal principal, string viewName)
	{
		string userName = principal.Identity.Name;
		return GetStore().Items<PersonalizedViewSettingsStorage>().FirstOrDefault((PersonalizedViewSettingsStorage settings) => settings.UserName == userName && settings.ViewName == viewName);
	}

	/// <summary>
	///       Loads the specified principal.
	///       </summary>
	/// <param name="principal">The principal.</param>
	/// <returns>All view settings for the specified user.</returns>
	public IEnumerable<PersonalizedViewSettings> Load(IPrincipal principal)
	{
		ArgumentNullException.ThrowIfNull(principal, "principal");
		IQueryable<PersonalizedViewSettingsStorage> queryable = from setting in GetStore().Items<PersonalizedViewSettingsStorage>()
			where setting.UserName == principal.Identity.Name
			select setting;
		PersonalizedViewSettings[] array = new PersonalizedViewSettings[queryable.Count()];
		int num = 0;
		foreach (PersonalizedViewSettingsStorage item in queryable)
		{
			array[num++] = item.ConvertToPersonalizedViewSettings(principal);
		}
		return array;
	}

	private DynamicDataStore GetStore()
	{
		return _ddsFactory.GetStore(typeof(PersonalizedViewSettingsStorage)) ?? _ddsFactory.CreateStore(typeof(PersonalizedViewSettingsStorage));
	}
}
