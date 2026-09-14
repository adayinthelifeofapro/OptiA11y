using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Extracts metadata for classes and properties and applies information from
///       system wide editor extenders registered in <see cref="P:EPiServer.Shell.ObjectEditing.ExtensibleMetadataProvider.MetadataHandlerRegistry" />.
///       </summary>
public class ExtensibleMetadataProvider : IModelMetadataProvider
{
	private readonly MetadataHandlerRegistry _handlerRegistry;

	private readonly LocalizationService _localizationService;

	private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;

	/// <summary>
	///       Gets the metadata handler registry.
	///       </summary>
	/// <value>The metadata handler registry.</value>
	public virtual MetadataHandlerRegistry MetadataHandlerRegistry => _handlerRegistry;

	/// <summary>
	///       Gets the default modelmetadata provider.
	///       </summary>
	/// <value>The default modelmetadata provider.</value>
	public IModelMetadataProvider DefaultProvider { get; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.ExtensibleMetadataProvider" /> class.
	///       </summary>
	/// <param name="metadataHandlerRegistry">The metadata registry.</param>
	/// <param name="localizationService">The localization service.</param>
	/// <param name="defaultProvider">The default Metadata provider.</param>
	/// <param name="validationAttributeAdapterProvider">The provider for supplying AttributeAdapters.</param>
	public ExtensibleMetadataProvider(MetadataHandlerRegistry metadataHandlerRegistry, LocalizationService localizationService, IModelMetadataProvider defaultProvider, IValidationAttributeAdapterProvider validationAttributeAdapterProvider)
	{
		_handlerRegistry = metadataHandlerRegistry;
		_localizationService = localizationService;
		DefaultProvider = defaultProvider;
		_validationAttributeAdapterProvider = validationAttributeAdapterProvider;
	}

	/// <summary>
	///       Method is required by interface IModelMetadataProvider. Use method GetExtendedMetadataForType(Type modelType, Func&lt;object&gt; modelAccessor) instead.
	///       </summary>
	/// <param name="modelType">
	/// </param>
	/// <returns>
	/// </returns>
	public ModelMetadata GetMetadataForType(Type modelType)
	{
		throw new NotImplementedException("Use method GetExtendedMetadataForType(Type modelType, Func<object> modelAccessor) instead");
	}

	/// <summary>
	///       Method is required by interface IModelMetadataProvider. Use method GetExtendedMetadataForProperties(ExtendedMetadata parent, object container, Type containerType, IMetadataProvider customProvider) instead.
	///       </summary>
	/// <param name="modelType">
	/// </param>
	/// <returns>
	/// </returns>
	public IEnumerable<ModelMetadata> GetMetadataForProperties(Type modelType)
	{
		throw new NotImplementedException("Use method GetExtendedMetadataForProperties(ExtendedMetadata parent, object container, Type containerType, IMetadataProvider customProvider) instead");
	}

	/// <summary>
	///       Replace required method GetMetadataForType(Type modelType)
	///       </summary>
	/// <param name="modelType">
	/// </param>
	/// <param name="modelAccessor">
	/// </param>
	/// <returns>
	/// </returns>
	public virtual ExtendedMetadata GetExtendedMetadataForType(Type modelType, Func<object> modelAccessor)
	{
		return GetExtendedMetadata(Attribute.GetCustomAttributes(modelType, inherit: true), null, modelAccessor, modelType, null);
	}

	/// <summary>
	///       Async version of <see cref="M:EPiServer.Shell.ObjectEditing.ExtensibleMetadataProvider.GetExtendedMetadataForType(System.Type,System.Func{System.Object})" />.
	///       </summary>
	/// <param name="modelType">
	/// </param>
	/// <param name="modelAccessor">
	/// </param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	/// <returns>
	/// </returns>
	public virtual Task<ExtendedMetadata> GetExtendedMetadataForTypeAsync(Type modelType, Func<object> modelAccessor, CancellationToken cancellationToken = default(CancellationToken))
	{
		return GetExtendedMetadataAsync(Attribute.GetCustomAttributes(modelType, inherit: true), null, modelAccessor, modelType, null, cancellationToken);
	}

	/// <summary>
	///       Replace required method GetMetadataForProperties(Type modelType)
	///       </summary>
	/// <param name="parent">The parent metadata.</param>
	/// <param name="container">The container.</param>
	/// <param name="containerType">Type of the container.</param>
	/// <param name="customProvider">The custom provider.</param>
	/// <returns>
	///       The metadata for the properties of a given object/type.
	///       </returns>
	[Obsolete("Use GetExtendedMetadataForPropertiesAsync instead.")]
	public IEnumerable<ExtendedMetadata> GetExtendedMetadataForProperties(ExtendedMetadata parent, object container, Type containerType, IMetadataProvider customProvider)
	{
		return Array.Empty<ExtendedMetadata>();
	}

	/// <summary>
	///       Async version of <see cref="M:EPiServer.Shell.ObjectEditing.ExtensibleMetadataProvider.GetExtendedMetadataForProperties(EPiServer.Shell.ObjectEditing.ExtendedMetadata,System.Object,System.Type,EPiServer.Shell.ObjectEditing.IMetadataProvider)" />.
	///       </summary>
	/// <param name="parent">The parent metadata.</param>
	/// <param name="container">The container.</param>
	/// <param name="containerType">Type of the container.</param>
	/// <param name="customProvider">The custom provider.</param>
	/// <param name="cancellationToken">A token to cancel the operation.</param>
	/// <returns>
	///       The metadata for the properties of a given object/type.
	///       </returns>
	public async Task<IEnumerable<ExtendedMetadata>> GetExtendedMetadataForPropertiesAsync(ExtendedMetadata parent, object container, Type containerType, IMetadataProvider customProvider, CancellationToken cancellationToken = default(CancellationToken))
	{
		IEnumerable<ExtendedMetadata> metadata = customProvider.GetMetadataForProperties(container, containerType);
		foreach (ExtendedMetadata propertyMetadata in metadata)
		{
			propertyMetadata.Parent = parent;
			propertyMetadata.CustomMetadataProvider = customProvider;
			IEnumerable<IMetadataHandler> metadataHandlers = ResolveMetadataHandlers(propertyMetadata);
			await ApplyExtendersToMetadataAsync(propertyMetadata, propertyMetadata.Attributes.OfType<Attribute>(), metadataHandlers, cancellationToken);
			ApplyMetadataAwareAttributes(propertyMetadata, propertyMetadata.Attributes.OfType<Attribute>());
		}
		return metadata;
	}

	/// <summary>
	///       Gets the metadata for the specified property.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	/// <param name="containerType">The type of the container.</param>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <param name="modelType">The type of the model.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The metadata for the property.</returns>
	public ExtendedMetadata GetExtendedMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
	{
		return GetExtendedMetadata(attributes, containerType, modelAccessor, modelType, propertyName, null);
	}

	/// <summary>
	///       Gets the metadata for the specified property.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	/// <param name="containerType">The type of the container.</param>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <param name="modelType">The type of the model.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <param name="uiHint">UI Hint</param>
	/// <returns>The metadata for the property.</returns>
	public ExtendedMetadata GetExtendedMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName, string uiHint)
	{
		var (extendedMetadata, metadataHandlers) = ResolveMetadataAndHandlers(attributes, containerType, modelAccessor, modelType, propertyName, uiHint);
		ApplyExtendersToMetadata(extendedMetadata, attributes, metadataHandlers);
		ApplyMetadataAwareAttributes(extendedMetadata, attributes);
		return extendedMetadata;
	}

	/// <summary>
	///       Async version of <see cref="M:EPiServer.Shell.ObjectEditing.ExtensibleMetadataProvider.GetExtendedMetadata(System.Collections.Generic.IEnumerable{System.Attribute},System.Type,System.Func{System.Object},System.Type,System.String)" />.
	///       </summary>
	public Task<ExtendedMetadata> GetExtendedMetadataAsync(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName, CancellationToken cancellationToken = default(CancellationToken))
	{
		return GetExtendedMetadataAsync(attributes, containerType, modelAccessor, modelType, propertyName, null, cancellationToken);
	}

	/// <summary>
	///       Async version of <see cref="M:EPiServer.Shell.ObjectEditing.ExtensibleMetadataProvider.GetExtendedMetadata(System.Collections.Generic.IEnumerable{System.Attribute},System.Type,System.Func{System.Object},System.Type,System.String,System.String)" />.
	///       </summary>
	public async Task<ExtendedMetadata> GetExtendedMetadataAsync(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName, string uiHint, CancellationToken cancellationToken = default(CancellationToken))
	{
		var (metadata, metadataHandlers) = ResolveMetadataAndHandlers(attributes, containerType, modelAccessor, modelType, propertyName, uiHint);
		await ApplyExtendersToMetadataAsync(metadata, attributes, metadataHandlers, cancellationToken);
		ApplyMetadataAwareAttributes(metadata, attributes);
		return metadata;
	}

	/// <summary>
	///       Internal creation of meta data that use the logic from the .NET class and extends it with additional EPiServer specific attributes.
	///       </summary>
	private ExtendedMetadata CreateExtendedMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
	{
		attributes.FirstOfType<DisplayColumnAttribute>();
		ModelMetadata modelMetadata = (string.IsNullOrEmpty(propertyName) ? DefaultProvider.GetMetadataForType(modelType) : ((!(containerType.GetProperty(propertyName) != null)) ? DefaultProvider.GetMetadataForType(modelType) : DefaultProvider.GetMetadataForProperty(containerType, propertyName)));
		ExtendedMetadata extendedMetadata = new ExtendedMetadata((modelMetadata as DefaultModelMetadata) ?? throw new InvalidOperationException("Can only extend metadata for DefaultModelMetadata"), _validationAttributeAdapterProvider, this, containerType, modelAccessor, _localizationService);
		extendedMetadata.InitializeFromAttributes(attributes);
		return extendedMetadata;
	}

	private IEnumerable<IMetadataHandler> ResolveMetadataHandlers(ExtendedMetadata propertyMetadata)
	{
		IEnumerable<IMetadataHandler> metadataHandlers = _handlerRegistry.GetMetadataHandlers(propertyMetadata.ModelType, propertyMetadata.UIHint);
		if (!metadataHandlers.Any())
		{
			metadataHandlers = _handlerRegistry.GetMetadataHandlers(propertyMetadata.ModelType);
		}
		if (!metadataHandlers.Any())
		{
			object model = propertyMetadata.Model;
			object obj = ((model is PropertyData) ? model : null);
			Type type = ((obj != null) ? ((PropertyData)obj).PropertyValueType : null);
			if (type != null)
			{
				metadataHandlers = _handlerRegistry.GetMetadataHandlers(type, propertyMetadata.UIHint);
				if (!metadataHandlers.Any())
				{
					metadataHandlers = _handlerRegistry.GetMetadataHandlers(type);
				}
			}
		}
		return metadataHandlers;
	}

	private (ExtendedMetadata metadata, IEnumerable<IMetadataHandler> metadataHandlers) ResolveMetadataAndHandlers(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName, string uiHint)
	{
		ExtendedMetadata extendedMetadata = null;
		IEnumerable<IMetadataHandler> enumerable = _handlerRegistry.GetMetadataHandlers(modelType, uiHint);
		if (enumerable != null && enumerable.FirstOrDefault((IMetadataHandler e) => e is IMetadataProvider) is IMetadataProvider metadataProvider)
		{
			extendedMetadata = metadataProvider.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
			extendedMetadata.CustomMetadataProvider = metadataProvider;
		}
		if (extendedMetadata == null)
		{
			extendedMetadata = CreateExtendedMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
		}
		if (!string.IsNullOrEmpty(extendedMetadata.UIHint))
		{
			IEnumerable<IMetadataHandler> metadataHandlers = _handlerRegistry.GetMetadataHandlers(modelType, extendedMetadata.UIHint);
			if (metadataHandlers.Any())
			{
				enumerable = metadataHandlers;
			}
		}
		return (metadata: extendedMetadata, metadataHandlers: enumerable);
	}

	private static void ApplyExtendersToMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes, IEnumerable<IMetadataHandler> metadataHandlers)
	{
		if (metadataHandlers == null)
		{
			return;
		}
		foreach (IMetadataExtender item in metadataHandlers.OfType<IMetadataExtender>())
		{
			item.ModifyMetadata(metadata, attributes);
		}
	}

	private static async Task ApplyExtendersToMetadataAsync(ExtendedMetadata metadata, IEnumerable<Attribute> attributes, IEnumerable<IMetadataHandler> metadataHandlers, CancellationToken cancellationToken)
	{
		if (metadataHandlers == null)
		{
			return;
		}
		foreach (IMetadataExtender item in metadataHandlers.OfType<IMetadataExtender>())
		{
			await item.ModifyMetadataAsync(metadata, attributes, cancellationToken);
		}
	}

	private static void ApplyMetadataAwareAttributes(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
	{
		foreach (IDisplayMetadataProvider item in attributes.OfType<IDisplayMetadataProvider>())
		{
			ModelMetadataIdentity key = ModelMetadataIdentity.ForType(metadata.ModelType);
			ModelAttributes attributesForType = ModelAttributes.GetAttributesForType(metadata.GetType());
			DisplayMetadataProviderContext displayMetadataProviderContext = new DisplayMetadataProviderContext(key, attributesForType);
			foreach (KeyValuePair<object, object> additionalValue in metadata.AdditionalValues)
			{
				displayMetadataProviderContext.DisplayMetadata.AdditionalValues.Add(additionalValue);
			}
			item.CreateDisplayMetadata(displayMetadataProviderContext);
		}
	}
}
