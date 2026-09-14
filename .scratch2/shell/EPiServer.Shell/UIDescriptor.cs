using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell;

/// <summary>
///       Generic version of ui descriptor to simplify setup.
///       </summary>
/// <typeparam name="T">
/// </typeparam>
public class UIDescriptor<T> : UIDescriptor
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptor`1" /> class.
	///       </summary>
	protected UIDescriptor()
		: base(typeof(T))
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptor`1" /> class.
	///       </summary>
	/// <param name="iconClass">Represents CSS class name which will be used to display an icon for the type.</param>
	protected UIDescriptor(string iconClass)
		: base(typeof(T), iconClass, new string[1] { typeof(T).FullName.ToLowerInvariant() })
	{
	}
}
/// <summary>
///       Represents a UI descriptor for a content type (which could be page, block, etc...), which will be used to render UI for a specific content.
///       </summary>
public class UIDescriptor
{
	private readonly LocalizationService _localizationService;

	private readonly IContentTypeRepository _contentTypeRepository;

	/// <summary>
	///       Gets or sets connected CLR type.
	///       </summary>
	[JsonIgnore]
	[IgnoreDataMember]
	public Type ForType { get; set; }

	/// <summary>
	///       Type's identifier. Used to replace CLR type's fully qualified name.
	///       </summary>
	public string TypeIdentifier { get; set; }

	/// <summary>
	///       Represents the Type name that should be presented to the user.
	///       Will return null if the requested type does not exist in the repository for some reason (interface, base content type class etc.)
	///       </summary>
	public string TypeName => ContentTypeLocalizationExtensions.GetContentTypeName(_localizationService, _contentTypeRepository.Load(ForType));

	/// <summary>
	///       Gets the base type identifier, that is: the closest parent in the inheritance chain.
	///       </summary>
	public string BaseTypeIdentifier { get; set; }

	/// <summary>
	///       Represents CSS class name which will be used to display an icon for the type.
	///       </summary>
	public string IconClass { get; set; }

	/// <summary>
	///       Represents the types that can be parents to this type. For instance a ContentFolder for the BlockData type.
	///       </summary>
	public IEnumerable<Type> ContainerTypes { get; set; }

	/// <summary>
	///       Gets or sets the disabled views.
	///       </summary>
	/// <value>
	///       The disabled views.
	///       </value>
	public ICollection<string> DisabledViews { get; protected set; }

	/// <summary>
	///       An array with the drag and drop types that the item type represents.
	///       </summary>
	public ICollection<string> DndTypes { get; private set; }

	/// <summary>
	///       The unique key for the child node to the contenttypes node.
	///       </summary>
	/// <remarks>
	///       This value will default to the class name with lower case.
	///       Set this property to the path of the XML element that contains the
	///       label and description elements in one of your localization providers.
	///       (for instance an xml file in the /lang directory.)
	///       </remarks>
	public string LanguageKey { get; set; }

	/// <summary>
	///       Gets or sets the default view for this UI descriptor.
	///       </summary>
	public string DefaultView { get; set; }

	/// <summary>
	///       Gets or sets if this view can be sticked so that if selected it would be propagated
	///       throughout page requests.
	///       </summary>
	public bool? EnableStickyView { get; set; }

	/// <summary>
	///       Gets or sets the view to be shown when creating content for this UI descriptor.
	///       </summary>
	public string CreateView { get; set; }

	/// <summary>
	///       Gets or sets the view to be shown after publishing content for this UI descriptor.
	///       </summary>
	public string PublishView { get; set; }

	/// <summary>
	///       Gets or sets the available views.
	///       </summary>
	/// <value>
	///       The available views.
	///       </value>
	public IEnumerable<ViewConfiguration> AvailableViews { get; private set; }

	/// <summary>
	///       Gets or sets the icon class for commands for this type.
	///       </summary>
	public string CommandIconClass { get; set; }

	/// <summary>
	///       Flag indicates creating content is primary type or not
	///       </summary>
	public bool IsPrimaryType { get; set; }

	/// <summary>
	///       Gets or sets a list of the base types and interfaces that the item type directly inherited from.
	///       </summary>
	public IEnumerable<string> BaseTypes { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptor" /> class.
	///       </summary>
	/// <param name="forType">The type to describe.</param>
	public UIDescriptor(Type forType)
		: this(forType, null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptor" /> class.
	///       </summary>
	/// <param name="forType">The described CLR type.</param>
	/// <param name="iconClass">Represents CSS class name which will be used to display an icon for the type.</param>
	protected UIDescriptor(Type forType, string iconClass)
		: this(forType, iconClass, new string[1] { forType.FullName.ToLowerInvariant() })
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptor" /> class.
	///       </summary>
	/// <param name="forType">The described CLR type.</param>
	/// <param name="iconClass">Represents CSS class name which will be used to display an icon for the type.</param>
	/// <param name="dndTypes">An array with the drag and drop types that the item type represents.</param>
	/// <remarks>Only use this overload if you want to specifically register custom drag and drop types.</remarks>
	protected UIDescriptor(Type forType, string iconClass, IEnumerable<string> dndTypes)
		: this(forType, iconClass, dndTypes, ServiceProviderExtensions.GetAllInstances<ViewConfiguration>(ServiceLocator.Current), LocalizationService.Current, ServiceProviderExtensions.GetInstance<IContentTypeRepository>(ServiceLocator.Current))
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptor" /> class.
	///       </summary>
	/// <param name="forType">The described CLR type.</param>
	/// <param name="iconClass">Represents CSS class name which will be used to display an icon for the type.</param>
	/// <param name="dndTypes">An array with the drag and drop types that the item type represents.</param>
	/// <param name="viewConfigurations">All view configurations</param>
	/// <param name="localizationService">The localization service to use for getting localizations</param>
	/// <param name="contentTypeRepository">Content type repository</param>
	/// <remarks>Only use this overload if you want to specifically register custom drag and drop types.</remarks>
	internal UIDescriptor(Type forType, string iconClass, IEnumerable<string> dndTypes, IEnumerable<ViewConfiguration> viewConfigurations, LocalizationService localizationService, IContentTypeRepository contentTypeRepository)
	{
		ForType = forType;
		TypeIdentifier = forType.FullName.ToLowerInvariant();
		LanguageKey = forType.Name.ToLowerInvariant();
		IconClass = iconClass;
		DndTypes = dndTypes.ToList();
		AvailableViews = ExtractAvailableViews(viewConfigurations);
		_localizationService = localizationService;
		_contentTypeRepository = contentTypeRepository;
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptor" /> class.
	///       </summary>
	/// <param name="forType">The described CLR type.</param>
	/// <param name="typeIdentifier">Type's identifier. Used to replace CLR type's fully qualified name.</param>
	/// <param name="iconClass">Represents CSS class name which will be used to display an icon for the type.</param>
	/// <param name="dndTypes">An array with the drag and drop types that the item type represents.</param>
	/// <param name="viewConfigurations">All view configurations</param>
	/// <param name="localizationService">The localization service to use for getting localizations</param>
	/// <param name="contentTypeRepository">Content type repository</param>
	internal UIDescriptor(Type forType, string typeIdentifier, string iconClass, IEnumerable<string> dndTypes, IEnumerable<ViewConfiguration> viewConfigurations, LocalizationService localizationService, IContentTypeRepository contentTypeRepository)
	{
		ForType = forType;
		TypeIdentifier = typeIdentifier;
		IconClass = iconClass;
		DndTypes = dndTypes.ToList();
		AvailableViews = ExtractAvailableViews(viewConfigurations);
		_localizationService = localizationService;
		_contentTypeRepository = contentTypeRepository;
	}

	/// <summary>
	///       Extracts the available views from view configurations.
	///       </summary>
	/// <param name="viewConfigurations">The view configurations.</param>
	/// <returns>
	/// </returns>
	private IEnumerable<ViewConfiguration> ExtractAvailableViews(IEnumerable<ViewConfiguration> viewConfigurations)
	{
		return from c in viewConfigurations.Where((ViewConfiguration view) => view.ForType == ForType).Select(delegate(ViewConfiguration view)
			{
				if (!string.IsNullOrEmpty(view.LanguagePath))
				{
					view.Name = _localizationService.GetString(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", view.LanguagePath, "name"));
					view.Description = _localizationService.GetString(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", view.LanguagePath, "description"));
				}
				return view;
			})
			orderby string.IsNullOrEmpty(c.Category), c.Category, c.SortOrder
			select c;
	}

	/// <summary>
	///       Add a view to the list of disabled views.
	///       </summary>
	/// <param name="viewName">
	/// </param>
	public void AddDisabledView(string viewName)
	{
		if (DisabledViews == null)
		{
			ICollection<string> collection = (DisabledViews = new List<string>());
		}
		DisabledViews.Add(viewName);
	}
}
