using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Provides metadata for a given type or property.
///       </summary>
public class ExtendedMetadata : DisplayMetadata
{
	public const string ExtendedMetadataDisplayKey = "epi:extendedmetadata";

	private IEnumerable<Attribute> _attributes;

	private IEnumerable<ExtendedMetadata> _properties;

	private readonly LocalizationService _localizationService;

	private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

	private List<IClientModelValidator> _validators;

	private readonly ModelValidationContext _validationContext;

	private readonly Type _modelType;

	private readonly Func<object> _modelAccessor;

	private readonly ExtensibleMetadataProvider _provider;

	[CompilerGenerated]
	private Type _003CRealModelType_003Ek__BackingField;

	private bool? _isReadOnly;

	private bool? _isRequired;

	public DefaultModelMetadata DefaultMetadata { get; }

	public virtual object Model { get; private set; }

	/// <summary>
	///       Gets or sets the Parent metadata.
	///       </summary>
	/// <value>
	///       The Parent.
	///       </value>
	public ExtendedMetadata Parent { get; set; }

	/// <summary>
	///       Gets or sets the hint which is used to find a specific metadata handler for this type and hint.
	///       </summary>
	/// <value>
	///       The hint.
	///       </value>
	public string UIHint { get; set; }

	/// <summary>
	///       Gets or sets the custom metadata provider.
	///       </summary>
	/// <value>The custom metadata provider.</value>
	public IMetadataProvider CustomMetadataProvider { get; set; }

	internal Type RealModelType
	{
		get
		{
			if (_003CRealModelType_003Ek__BackingField == null)
			{
				_003CRealModelType_003Ek__BackingField = _modelType;
				object obj = _modelAccessor();
				if (obj != null && !IsNullable(obj.GetType()))
				{
					_003CRealModelType_003Ek__BackingField = obj.GetType();
				}
			}
			return _003CRealModelType_003Ek__BackingField;
		}
	}

	/// <summary>
	///       Gets a collection of model metadata objects that describe the properties of the model.
	///       </summary>
	/// <value>
	/// </value>
	/// <returns>A collection of model metadata objects that describe the properties of the model.</returns>
	[Obsolete("This property is obsolete and will be removed in CMS14. Please use GetPropertiesAsync instead.")]
	public virtual IEnumerable<ExtendedMetadata> Properties => Array.Empty<ExtendedMetadata>();

	/// <summary>
	///       Gets the mappings between property names.
	///       </summary>
	public IEnumerable<PropertyMapping> MappedProperties
	{
		get
		{
			if (CustomMetadataProvider is IMetadataPropertyMappingProvider metadataPropertyMappingProvider)
			{
				return metadataPropertyMappingProvider.GetPropertyMappings(_modelAccessor());
			}
			return Enumerable.Empty<PropertyMapping>();
		}
	}

	/// <summary>
	///       Gets or sets the name of the group.
	///       </summary>
	/// <value>
	///       The name of the group.
	///       </value>
	public string GroupName { get; set; }

	/// <summary>
	///       Gets or sets the client editing class.
	///       </summary>
	/// <value>
	///       The client editing class.
	///       </value>
	public string ClientEditingClass { get; set; }

	/// <summary>
	///       Gets or sets the client editing package.
	///       </summary>
	/// <value>
	///       The client editing package.
	///       </value>
	public string ClientEditingPackage { get; set; }

	/// <summary>
	///       Gets or sets the custom editor settings.
	///       </summary>
	/// <value>The custom editor settings.</value>
	/// <remarks>This might include information about custom editor wrappers.</remarks>
	public IDictionary<string, object> CustomEditorSettings { get; private set; }

	/// <summary>
	///       Gets or sets the layout class.
	///       </summary>
	/// <value>
	///       The layout class.
	///       </value>
	public string LayoutClass { get; set; }

	/// <summary>
	///       Gets or sets the initial value.
	///       </summary>
	/// <value>
	///       The initial value.
	///       </value>
	public object InitialValue { get; set; }

	/// <summary>
	///       Gets or sets the type of the selection factory.
	///       </summary>
	/// <value>
	///       The type of the selection factory.
	///       </value>
	public Type SelectionFactoryType { get; set; }

	/// <summary>
	///       Gets or sets the editor configuraion.
	///       </summary>
	/// <value>
	///       The editor configuraion.
	///       </value>
	public IDictionary<string, object> EditorConfiguration { get; private set; }

	/// <summary>
	///       Gets or sets the configuration data for the overlay.
	///       </summary>
	/// <value>
	///       The overlay configuraion.
	///       </value>
	public IDictionary<string, object> OverlayConfiguration { get; private set; }

	/// <summary>
	///       Gets or sets the <see cref="P:EPiServer.Shell.ObjectEditing.ExtendedMetadata.GroupSettings" /> information for the current property.
	///       </summary>
	/// <value>The group settings.</value>
	/// <remarks>This can only be defined on property level. If several properties specify group settings for the same group an exception will be thrown.</remarks>
	public GroupSettings GroupSettings { get; set; }

	public BindingMetadata BindingMetadata => DefaultMetadata.BindingMetadata;

	public ValidationMetadata ValidationMetadata
	{
		get
		{
			try
			{
				return DefaultMetadata.ValidationMetadata;
			}
			catch (ArgumentNullException)
			{
				return new ValidationMetadata();
			}
		}
	}

	/// <summary>
	///       Gets or sets the attributes.
	///       </summary>
	public virtual IEnumerable<Attribute> Attributes
	{
		get
		{
			if (_attributes == null)
			{
				_attributes = Enumerable.Empty<Attribute>();
			}
			return _attributes;
		}
		private set
		{
			_attributes = value;
		}
	}

	public new string DisplayName
	{
		get
		{
			return DefaultMetadata.DisplayName;
		}
		set
		{
			DefaultMetadata.DisplayMetadata.DisplayName = () => value;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return _isReadOnly ?? DefaultMetadata.IsReadOnly;
		}
		set
		{
			_isReadOnly = value;
		}
	}

	public bool IsRequired
	{
		get
		{
			return _isRequired ?? DefaultMetadata.IsRequired;
		}
		set
		{
			_isRequired = value;
		}
	}

	public string PropertyName => DefaultMetadata.PropertyName;

	public virtual Type ModelType => DefaultMetadata.ModelType;

	public Type ContainerType => DefaultMetadata.ContainerType;

	public bool IsNullableValueType => DefaultMetadata.IsNullableValueType;

	public bool IsComplexType => DefaultMetadata.IsComplexType;

	public ExtendedMetadata(DefaultModelMetadata defaultMetadata, IValidationAttributeAdapterProvider validationAttributeAdapterProvider, ExtensibleMetadataProvider provider, Type containerType = null, Func<object> modelAccessor = null, LocalizationService localizationService = null)
	{
		DefaultMetadata = defaultMetadata;
		_modelType = defaultMetadata.ModelType;
		CustomEditorSettings = new Dictionary<string, object>();
		EditorConfiguration = new Dictionary<string, object>();
		OverlayConfiguration = new Dictionary<string, object>();
		_localizationService = localizationService ?? LocalizationService.Current;
		_modelAccessor = modelAccessor ?? ((Func<object>)(() => (object)null));
		_provider = provider;
		_validationAttributeAdapterProvider = validationAttributeAdapterProvider;
		_validationContext = new ModelValidationContext(new ActionContext(), defaultMetadata, provider, null, null);
		base.AdditionalValues["epi:extendedmetadata"] = this;
		Model = _modelAccessor();
	}

	public void SetModel(object model)
	{
		Model = model;
	}

	public bool IsNullable(Type type)
	{
		return Nullable.GetUnderlyingType(type) != null;
	}

	/// <summary>
	///       Gets the properties using the async metadata extension path
	///       so that <see cref="M:EPiServer.Shell.ObjectEditing.IMetadataExtender.ModifyMetadataAsync(EPiServer.Shell.ObjectEditing.ExtendedMetadata,System.Collections.Generic.IEnumerable{System.Attribute},System.Threading.CancellationToken)" /> is called for each property.
	///       </summary>
	public virtual async Task<IEnumerable<ExtendedMetadata>> GetPropertiesAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_properties != null)
		{
			return _properties;
		}
		if (CustomMetadataProvider != null)
		{
			_properties = await _provider.GetExtendedMetadataForPropertiesAsync(this, _modelAccessor(), RealModelType, CustomMetadataProvider, cancellationToken);
		}
		else
		{
			List<ExtendedMetadata> result = new List<ExtendedMetadata>();
			foreach (ModelMetadata property in DefaultMetadata.Properties)
			{
				DefaultModelMetadata defaultModelMetadata = property as DefaultModelMetadata;
				result.Add(await _provider.GetExtendedMetadataAsync(defaultModelMetadata.Attributes.Attributes.OfType<Attribute>(), property.ContainerType, null, property.ModelType, property.PropertyName, cancellationToken));
			}
			_properties = result;
		}
		return _properties;
	}

	private void ReadSettingsFromSystemAttribute(SystemPropertyAttribute systemPropertyAttribute)
	{
		if (systemPropertyAttribute != null)
		{
			base.ShowForDisplay = false;
		}
	}

	/// <summary>
	///       Generate widget setting string
	///       </summary>
	/// <returns>Widget setting string</returns>
	public Dictionary<string, object> GetEditorSettings(LocalizationService localizationService)
	{
		Dictionary<string, object> dictionary = ((EditorConfiguration != null) ? new Dictionary<string, object>(EditorConfiguration) : new Dictionary<string, object>());
		dictionary["label"] = ((DisplayName != null) ? localizationService.GetTranslationIfNeeded(DisplayName.ToString()) : DefaultMetadata.PropertyName);
		if (IsRequired)
		{
			dictionary["required"] = true;
		}
		string requiredMessage = GetRequiredMessage(localizationService);
		if (!string.IsNullOrEmpty(requiredMessage))
		{
			dictionary["missingMessage"] = requiredMessage;
		}
		string invalidMessage = GetInvalidMessage(localizationService);
		if (!string.IsNullOrEmpty(invalidMessage))
		{
			dictionary["invalidMessage"] = invalidMessage;
		}
		foreach (KeyValuePair<string, object> validationSetting in GetValidationSettings(localizationService, dictionary))
		{
			dictionary[validationSetting.Key] = validationSetting.Value;
		}
		if (IsReadOnly)
		{
			dictionary["readOnly"] = true;
		}
		if (base.Placeholder != null && !string.IsNullOrEmpty(base.Placeholder()))
		{
			dictionary["placeHolder"] = localizationService.GetTranslationIfNeeded(base.Placeholder());
		}
		return dictionary;
	}

	/// <summary>
	///       Take error message from Required validator to assign to widget's missingMessage
	///       </summary>
	/// <returns>
	/// </returns>
	private string GetRequiredMessage(LocalizationService localizationService)
	{
		if (IsRequired)
		{
			AttributeAdapterBase<RequiredAttribute> attributeAdapter = GetAttributeAdapter<RequiredAttribute>();
			if (attributeAdapter == null)
			{
				return null;
			}
			RequiredAttribute requiredAttribute = Attributes.FirstOfType<RequiredAttribute>();
			if (requiredAttribute != null && string.IsNullOrWhiteSpace(requiredAttribute.ErrorMessage) && string.IsNullOrWhiteSpace(requiredAttribute.ErrorMessageResourceName))
			{
				string arg = DefaultMetadata.PropertyName;
				if (DisplayName != null)
				{
					arg = DisplayName.ToString();
				}
				return string.Format(CultureInfo.CurrentCulture, localizationService.GetString("/episerver/shared/validation/required"), arg);
			}
			return localizationService.GetTranslationIfNeeded(attributeAdapter.GetErrorMessage(_validationContext));
		}
		return null;
	}

	/// <summary>
	///       Concatenate invalid message from all validators
	///       </summary>
	/// <returns>
	/// </returns>
	private string GetInvalidMessage(LocalizationService localizationService)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = true;
		AttributeAdapterBase<RegularExpressionAttribute> attributeAdapter = GetAttributeAdapter<RegularExpressionAttribute>();
		AttributeAdapterBase<StringLengthAttribute> attributeAdapter2 = GetAttributeAdapter<StringLengthAttribute>();
		if (attributeAdapter != null)
		{
			stringBuilder.Append(localizationService.GetTranslationIfNeeded(attributeAdapter.GetErrorMessage(_validationContext)));
			flag = false;
		}
		else if (attributeAdapter2 != null)
		{
			stringBuilder.Append(localizationService.GetTranslationIfNeeded(attributeAdapter2.GetErrorMessage(_validationContext)));
			flag = false;
		}
		AttributeAdapterBase<RangeAttribute> attributeAdapter3 = GetAttributeAdapter<RangeAttribute>();
		if (attributeAdapter3 != null)
		{
			if (!flag)
			{
				stringBuilder.Append("<br />");
			}
			stringBuilder.Append(localizationService.GetTranslationIfNeeded(attributeAdapter3.GetErrorMessage(_validationContext)));
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	///       Try to convert MVC validation rules into dojo's regEx pattern
	///       and update constraints.
	///       </summary>
	/// <returns>A dictionary containing dojo settings</returns>
	private Dictionary<string, object> GetValidationSettings(LocalizationService localizationService, Dictionary<string, object> editorSettings)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		AttributeAdapterBase<RegularExpressionAttribute> attributeAdapter = GetAttributeAdapter<RegularExpressionAttribute>();
		if (attributeAdapter != null && attributeAdapter.Attribute.Pattern != null)
		{
			dictionary.Add("pattern", attributeAdapter.Attribute.Pattern);
		}
		else
		{
			AttributeAdapterBase<StringLengthAttribute> attributeAdapter2 = GetAttributeAdapter<StringLengthAttribute>();
			if (attributeAdapter2 != null)
			{
				dictionary.Add("pattern", string.Format(CultureInfo.InvariantCulture, "^.{{{0},{1}}}$", attributeAdapter2.Attribute.MinimumLength, attributeAdapter2.Attribute.MaximumLength));
				dictionary.Add("maxLength", attributeAdapter2.Attribute.MaximumLength);
			}
		}
		AttributeAdapterBase<RangeAttribute> attributeAdapter3 = GetAttributeAdapter<RangeAttribute>();
		if (attributeAdapter3 != null && attributeAdapter3.Attribute.Minimum != null && attributeAdapter3.Attribute.Maximum != null)
		{
			if (editorSettings.TryGetValue("constraints", out var value) && value is Dictionary<string, object> dictionary2)
			{
				dictionary.Add("constraints", new Dictionary<string, object>(dictionary2)
				{
					["min"] = attributeAdapter3.Attribute.Minimum,
					["max"] = attributeAdapter3.Attribute.Maximum
				});
			}
			else
			{
				dictionary.Add("constraints", new
				{
					min = attributeAdapter3.Attribute.Minimum,
					max = attributeAdapter3.Attribute.Maximum
				});
			}
			dictionary.Add("rangeMessage", localizationService.GetTranslationIfNeeded(attributeAdapter3.GetErrorMessage(_validationContext)));
		}
		return dictionary;
	}

	/// <summary>
	///       Adds the validator to the list of validator.
	///       </summary>
	/// <param name="validator">The validator to add.</param>
	public void AddValidator(IClientModelValidator validator)
	{
		if (_validators == null)
		{
			_validators = new List<IClientModelValidator> { validator };
		}
		else
		{
			_validators.Add(validator);
		}
	}

	/// <summary>
	///       Gets the list of validator.
	///       </summary>
	/// <returns>List of validator for this metadata</returns>
	public IEnumerable<IClientModelValidator> GetValidators()
	{
		List<IClientModelValidator> list = new List<IClientModelValidator>();
		foreach (object validatorMetadatum in DefaultMetadata.ValidatorMetadata)
		{
			if (new ValidatorItem(validatorMetadatum).ValidatorMetadata is ValidationAttribute attribute)
			{
				IAttributeAdapter attributeAdapter = _validationAttributeAdapterProvider.GetAttributeAdapter(attribute, null);
				list.Add(attributeAdapter);
			}
		}
		if (_validators != null)
		{
			list.AddRange(_validators);
		}
		return list;
	}

	/// <summary>
	///       Gets the client validator attribute adapter.
	///       </summary>
	/// <returns>
	/// </returns>
	public AttributeAdapterBase<T> GetAttributeAdapter<T>() where T : ValidationAttribute
	{
		return GetValidators().FirstOfType<AttributeAdapterBase<T>>();
	}

	/// <summary>
	///       Initializes metadata values from data annotation attributes.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	public virtual void InitializeFromAttributes(IEnumerable<Attribute> attributes)
	{
		Attributes = attributes;
		ReadSettingsFromEditorDescriptorAttribute(attributes.FirstOfType<EditorDescriptorAttribute>());
		ReadSettingsFromHiddenInputAttribute(attributes.FirstOfType<HiddenInputAttribute>());
		ReadSettingsFromUIHintAttributes(attributes.OfType<UIHintAttribute>());
		DataTypeAttribute dataTypeAttribute = attributes.FirstOfType<DataTypeAttribute>();
		ReadSettingsFromDataTypeAttribute(dataTypeAttribute);
		ReadSettingsFromDisplayFormatAttribute(attributes.FirstOfType<DisplayFormatAttribute>(), dataTypeAttribute);
		ReadSettingsFromEditableAttributes(attributes);
		ReadSettingsFromScaffoldColumnAttribute(attributes.FirstOfType<ScaffoldColumnAttribute>());
		ReadSettingsFromSystemAttribute(attributes.FirstOfType<SystemPropertyAttribute>());
		ReadSettingsFromDisplayAttributes(attributes);
		ReadSettingsFromRequiredAttribute(attributes.FirstOfType<RequiredAttribute>());
		ReadSettingsFromClientSideEditorAttribute(attributes.FirstOfType<ClientEditorAttribute>());
		ReadSettingsFromGroupSettingsAttribute(attributes.FirstOfType<GroupSettingsAttribute>());
		ReadSettingsFromHintAttributes(attributes);
	}

	/// <summary>
	///       Extracts the settings from hidden input attribute.
	///       </summary>
	/// <param name="hiddenInputAttribute">The hidden input attribute.</param>
	protected virtual void ReadSettingsFromHiddenInputAttribute(HiddenInputAttribute hiddenInputAttribute)
	{
		if (hiddenInputAttribute != null)
		{
			base.TemplateHint = "HiddenInput";
			EditorConfiguration["Type"] = "hidden";
			base.HideSurroundingHtml = !hiddenInputAttribute.DisplayValue;
		}
	}

	/// <summary>
	///       Reads the settings from UI hint attribute.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	protected virtual void ReadSettingsFromUIHintAttributes(IEnumerable<UIHintAttribute> attributes)
	{
		UIHintAttribute uIHintAttribute = attributes.FirstOrDefault((UIHintAttribute a) => string.Equals(a.PresentationLayer, "MVC", StringComparison.OrdinalIgnoreCase)) ?? attributes.FirstOrDefault((UIHintAttribute a) => string.IsNullOrEmpty(a.PresentationLayer));
		if (uIHintAttribute != null)
		{
			base.TemplateHint = uIHintAttribute.UIHint;
		}
	}

	/// <summary>
	///       Reads the settings from editable attribute.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	protected virtual void ReadSettingsFromEditableAttributes(IEnumerable<Attribute> attributes)
	{
		ArgumentNullException.ThrowIfNull(attributes, "attributes");
		EditableAttribute editableAttribute = attributes.OfType<EditableAttribute>().FirstOrDefault();
		if (editableAttribute != null)
		{
			BindingMetadata.IsReadOnly = !editableAttribute.AllowEdit;
			return;
		}
		ReadOnlyAttribute readOnlyAttribute = attributes.OfType<ReadOnlyAttribute>().FirstOrDefault();
		if (readOnlyAttribute != null)
		{
			BindingMetadata.IsReadOnly = readOnlyAttribute.IsReadOnly;
		}
	}

	/// <summary>
	///       Reads the settings from scaffold column attribute.
	///       </summary>
	/// <param name="scaffoldColumnAttribute">The scaffold column attribute.</param>
	protected virtual void ReadSettingsFromScaffoldColumnAttribute(ScaffoldColumnAttribute scaffoldColumnAttribute)
	{
		if (scaffoldColumnAttribute != null)
		{
			bool showForDisplay = (base.ShowForEdit = scaffoldColumnAttribute.Scaffold);
			base.ShowForDisplay = showForDisplay;
		}
	}

	/// <summary>
	///       Reads the settings from display attribute.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	protected virtual void ReadSettingsFromDisplayAttributes(IEnumerable<Attribute> attributes)
	{
		ArgumentNullException.ThrowIfNull(attributes, "attributes");
		DisplayAttribute displayAttribute = attributes.FirstOfType<DisplayAttribute>();
		string text = null;
		if (displayAttribute != null)
		{
			base.Description = displayAttribute.GetDescription;
			base.SimpleDisplayProperty = displayAttribute.GetShortName();
			base.Placeholder = displayAttribute.GetPrompt;
			base.Order = displayAttribute.GetOrder() ?? ModelMetadata.DefaultOrder;
			GroupName = displayAttribute.GetGroupName();
			text = displayAttribute.GetName();
		}
		if (text != null)
		{
			DisplayName = text;
		}
		else
		{
			DisplayNameAttribute displayNameAttribute = attributes.FirstOfType<DisplayNameAttribute>();
			if (displayNameAttribute != null)
			{
				DisplayName = displayNameAttribute.DisplayName;
			}
		}
		if (DisplayName != null)
		{
			string translationIfNeeded = _localizationService.GetTranslationIfNeeded(DisplayName.ToString());
			DisplayName = translationIfNeeded;
		}
	}

	/// <summary>
	///       Reads the settings from required attribute.
	///       </summary>
	/// <param name="requiredAttribute">The required attribute.</param>
	protected virtual void ReadSettingsFromRequiredAttribute(RequiredAttribute requiredAttribute)
	{
		if (requiredAttribute != null)
		{
			ValidationMetadata.IsRequired = true;
		}
	}

	/// <summary>
	///       Reads the settings from client side editor attribute.
	///       </summary>
	/// <param name="editorAttribute">The editor attribute.</param>
	protected virtual void ReadSettingsFromClientSideEditorAttribute(ClientEditorAttribute editorAttribute)
	{
		if (editorAttribute == null)
		{
			return;
		}
		if (!string.IsNullOrEmpty(editorAttribute.ClientEditingClass))
		{
			ClientEditingClass = editorAttribute.ClientEditingClass;
		}
		if (!string.IsNullOrEmpty(editorAttribute.ClientEditingPackage))
		{
			ClientEditingPackage = editorAttribute.ClientEditingPackage;
		}
		if (!string.IsNullOrEmpty(editorAttribute.DefaultValue))
		{
			InitialValue = editorAttribute.DefaultValue;
		}
		if (!string.IsNullOrEmpty(editorAttribute.EditorConfiguration))
		{
			EditorConfiguration = (string.IsNullOrEmpty(editorAttribute.EditorConfiguration) ? new Dictionary<string, object>() : JsonSerializer.Deserialize<Dictionary<string, object>>(editorAttribute.EditorConfiguration));
		}
		if (editorAttribute.IsJavascriptModule)
		{
			EditorConfiguration["IsJavascriptModule"] = true;
		}
		if (!string.IsNullOrEmpty(editorAttribute.OverlayConfiguration))
		{
			OverlayConfiguration = (string.IsNullOrEmpty(editorAttribute.OverlayConfiguration) ? new Dictionary<string, object>() : JsonSerializer.Deserialize<Dictionary<string, object>>(editorAttribute.OverlayConfiguration));
		}
		if (editorAttribute.SelectionFactoryType != null)
		{
			if (!typeof(ISelectionFactory).IsAssignableFrom(editorAttribute.SelectionFactoryType))
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "{0} type does not implement ISelectionFactory", editorAttribute.SelectionFactoryType));
			}
			SelectionFactoryType = editorAttribute.SelectionFactoryType;
		}
		if (!string.IsNullOrEmpty(editorAttribute.LayoutClass))
		{
			LayoutClass = editorAttribute.LayoutClass;
		}
	}

	/// <summary>
	///       Reads the settings from group settings attribute.
	///       </summary>
	/// <param name="groupSettingsAttribute">The group settings attribute.</param>
	protected void ReadSettingsFromGroupSettingsAttribute(GroupSettingsAttribute groupSettingsAttribute)
	{
		if (groupSettingsAttribute != null)
		{
			GroupSettings = new GroupSettings
			{
				Name = groupSettingsAttribute.Name,
				Title = groupSettingsAttribute.Title,
				ClientLayoutClass = groupSettingsAttribute.ClientLayoutClass
			};
		}
	}

	/// <summary>
	///       Reads the settings from data type override attribute.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	protected void ReadSettingsFromHintAttributes(IEnumerable<Attribute> attributes)
	{
		ArgumentNullException.ThrowIfNull(attributes, "attributes");
		IEnumerable<UIHintAttribute> source = attributes.OfType<UIHintAttribute>();
		UIHintAttribute uIHintAttribute = source.FirstOrDefault((UIHintAttribute a) => string.Equals(a.PresentationLayer, "edit", StringComparison.OrdinalIgnoreCase));
		if (uIHintAttribute != null)
		{
			UIHint = uIHintAttribute.UIHint;
			return;
		}
		UIHintAttribute uIHintAttribute2 = source.FirstOrDefault((UIHintAttribute a) => string.IsNullOrEmpty(a.PresentationLayer));
		if (uIHintAttribute2 != null)
		{
			UIHint = uIHintAttribute2.UIHint;
			return;
		}
		EditorHintAttribute val = attributes.OfType<EditorHintAttribute>().FirstOrDefault();
		if (val != null)
		{
			UIHint = val.Hint;
		}
	}

	/// <summary>
	///       Reads the settings from data type attribute.
	///       </summary>
	/// <param name="attribute">The attribute.</param>
	protected virtual void ReadSettingsFromDataTypeAttribute(DataTypeAttribute attribute)
	{
		if (attribute != null)
		{
			base.DataTypeName = attribute.GetDataTypeName();
		}
	}

	/// <summary>
	///       Reads the settings from display format attribute.
	///       </summary>
	/// <param name="displayFormatAttribute">The display format attribute.</param>
	/// <param name="dataTypeAttribute">The data type attribute.</param>
	protected virtual void ReadSettingsFromDisplayFormatAttribute(DisplayFormatAttribute displayFormatAttribute, DataTypeAttribute dataTypeAttribute)
	{
		if (displayFormatAttribute == null && dataTypeAttribute != null)
		{
			displayFormatAttribute = dataTypeAttribute.DisplayFormat;
		}
		if (displayFormatAttribute != null)
		{
			base.NullDisplayText = displayFormatAttribute.NullDisplayText;
			base.DisplayFormatString = displayFormatAttribute.DataFormatString;
			base.ConvertEmptyStringToNull = displayFormatAttribute.ConvertEmptyStringToNull;
			if (displayFormatAttribute.ApplyFormatInEditMode)
			{
				base.EditFormatString = displayFormatAttribute.DataFormatString;
			}
			if (!displayFormatAttribute.HtmlEncode && string.IsNullOrWhiteSpace(base.DataTypeName))
			{
				base.DataTypeName = DataType.Html.ToString();
			}
		}
	}

	/// <summary>
	///       Reads the settings from editor descriptor attribute.
	///       </summary>
	protected virtual void ReadSettingsFromEditorDescriptorAttribute(EditorDescriptorAttribute editorDescriptorAttribute)
	{
		if (editorDescriptorAttribute != null && !(editorDescriptorAttribute.EditorDescriptorType == null))
		{
			((Activator.CreateInstance(editorDescriptorAttribute.EditorDescriptorType) as IMetadataExtender) ?? throw new InvalidCastException("Specified type does not implement IMetadataExtender")).ModifyMetadata(this, null);
		}
	}
}
