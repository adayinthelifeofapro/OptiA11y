using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Contains personalized settings for a view for a certain user.
///       </summary>
public class PersonalizedViewSettings
{
	[CompilerGenerated]
	private IList<IContainer> _003CCustomizedContainers_003Ek__BackingField;

	/// <summary>
	///       Id for the personalized settings.
	///       </summary>
	public Guid Id { get; set; }

	/// <summary>
	///       Gets or sets the name of the view.
	///       </summary>
	/// <value>The name of the view.</value>
	public string ViewName { get; set; }

	/// <summary>
	///       Gets or sets the username for these settings.
	///       </summary>
	/// <value>The username for these settings.</value>
	public string UserName { get; set; }

	/// <summary>
	///       Gets or sets the containers that has been customized by the user.
	///       </summary>
	/// <value>The containers that has been customized by the user.</value>
	public IList<IContainer> CustomizedContainers
	{
		get
		{
			if (_003CCustomizedContainers_003Ek__BackingField == null)
			{
				_003CCustomizedContainers_003Ek__BackingField = new List<IContainer>();
			}
			return _003CCustomizedContainers_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCustomizedContainers_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" /> class.
	///       </summary>
	public PersonalizedViewSettings()
	{
		Id = Guid.NewGuid();
		CustomizedContainers = new List<IContainer>();
	}

	/// <summary>
	///       Returns a <see cref="T:EPiServer.Shell.Storage.ComponentData" /> of a component with <paramref name="storedComponentId" /> from the <paramref name="containers" /> collection
	///       </summary>
	/// <param name="containers">The containers.</param>
	/// <param name="storedComponentId">The stored component id to search for</param>
	/// <returns>
	/// </returns>
	public static IComponent GetComponent(IEnumerable<IComponent> containers, Guid storedComponentId)
	{
		if (storedComponentId == Guid.Empty)
		{
			return null;
		}
		foreach (IComponent container2 in containers)
		{
			if (container2.Id == storedComponentId)
			{
				return container2;
			}
			if (container2 is IContainer container)
			{
				IComponent component = GetComponent(container.Components, storedComponentId);
				if (component != null)
				{
					return component;
				}
			}
		}
		return null;
	}

	/// <summary>
	///       Locates a <see cref="T:EPiServer.Shell.Storage.ComponentData" /> by its id within a collection of <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" />.
	///       </summary>
	/// <param name="viewSettings">The collection of view settings in which to search for the component.</param>
	/// <param name="componentId">The id of the component to locate.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.Storage.ComponentData" /> settings object if a natch was found; otherwies <c>null</c></returns>
	public static IComponent FindComponentById(IEnumerable<PersonalizedViewSettings> viewSettings, Guid componentId)
	{
		PersonalizedViewSettings settings;
		return FindComponentById(viewSettings, componentId, out settings);
	}

	/// <summary>
	///       Locates a <see cref="T:EPiServer.Shell.Storage.ComponentData" /> by its id within a collection of <see cref="T:EPiServer.Shell.ViewComposition.PersonalizedViewSettings" />.
	///       </summary>
	/// <param name="viewSettings">The collection of view settings in which to search for the component.</param>
	/// <param name="componentId">The id of the component to locate.</param>
	/// <param name="settings">The view settings in which the component was found.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.Storage.ComponentData" /> settings object if a natch was found; otherwies <c>null</c></returns>
	public static IComponent FindComponentById(IEnumerable<PersonalizedViewSettings> viewSettings, Guid componentId, out PersonalizedViewSettings settings)
	{
		foreach (PersonalizedViewSettings viewSetting in viewSettings)
		{
			IComponent component = GetComponent(viewSetting.CustomizedContainers, componentId);
			if (component != null)
			{
				settings = viewSetting;
				return component;
			}
		}
		settings = null;
		return null;
	}

	/// <summary>
	///       Finds the container that matches the given id or null if no matching container exists.
	///       </summary>
	/// <param name="id">The id to match.</param>
	/// <param name="personalizationContainer">The personalization container that the component exists in if one exists.</param>
	/// <returns>
	///       The component that matches the given id or null if no matching container exists.
	///       </returns>
	public IComponent FindComponentById(Guid id, out IContainer personalizationContainer)
	{
		personalizationContainer = null;
		foreach (IContainer customizedContainer in CustomizedContainers)
		{
			IComponent component = customizedContainer.FindComponentById(id, ref personalizationContainer);
			if (component != null)
			{
				if (customizedContainer.ContainerType == ContainerType.User)
				{
					personalizationContainer = customizedContainer;
				}
				return component;
			}
		}
		return null;
	}
}
