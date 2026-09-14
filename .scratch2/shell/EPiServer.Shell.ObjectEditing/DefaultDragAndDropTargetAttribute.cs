using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Set this to define which property that should accept drag and drop operations for a complex object in the overlay.
///       </summary>
/// <remarks>An example of usage is to set this attribute on an image for a block containing an image and text. The
///       intended usage is to define this on a single property on a model. If this attribute has been defined on several
///       properties the last property will override the configuration defined by any other attributes on the same model.</remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class DefaultDragAndDropTargetAttribute : Attribute, IDisplayMetadataProvider, IMetadataDetailsProvider
{
	internal Injected<IClientModelValidatorProvider> ClientModelValidatorProvider;

	internal Injected<IServiceProvider> ServiceProvider;

	/// <summary>
	/// </summary>
	/// <param name="context">
	/// </param>
	public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
	{
		if (!(context.DisplayMetadata.AdditionalValues["epi:extendedmetadata"] is ExtendedMetadata extendedMetadata))
		{
			return;
		}
		if (extendedMetadata.OverlayConfiguration.TryGetValue("AllowedDndTypes", out var value))
		{
			extendedMetadata.Parent.OverlayConfiguration["AllowedDndTypes"] = value;
		}
		if (extendedMetadata.OverlayConfiguration.TryGetValue("dndSourcePropertyName", out var value2))
		{
			extendedMetadata.Parent.OverlayConfiguration["dndSourcePropertyName"] = value2;
		}
		extendedMetadata.Parent.OverlayConfiguration["dndTargetPropertyName"] = extendedMetadata.PropertyName;
		if (extendedMetadata.OverlayConfiguration.TryGetValue("dndTargetPropertyAllowMultiple", out var value3))
		{
			extendedMetadata.Parent.OverlayConfiguration["dndTargetPropertyAllowMultiple"] = value3;
			return;
		}
		Type modelType = extendedMetadata.ModelType;
		if ((modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(IList<>)) || modelType.GetInterfaces().Any((Type i) => i == typeof(IList)))
		{
			extendedMetadata.Parent.OverlayConfiguration["dndTargetPropertyAllowMultiple"] = true;
		}
	}

	/// <summary>
	///       When implemented in a class, provides metadata to the model metadata creation process.
	///       </summary>
	/// <param name="extendedMetadata">The model metadata.</param>
	public void OnMetadataCreated(ExtendedMetadata extendedMetadata)
	{
		if (extendedMetadata == null)
		{
			return;
		}
		if (extendedMetadata.OverlayConfiguration.TryGetValue("AllowedDndTypes", out var value))
		{
			extendedMetadata.Parent.OverlayConfiguration["AllowedDndTypes"] = value;
		}
		if (extendedMetadata.OverlayConfiguration.TryGetValue("dndSourcePropertyName", out var value2))
		{
			extendedMetadata.Parent.OverlayConfiguration["dndSourcePropertyName"] = value2;
		}
		extendedMetadata.Parent.OverlayConfiguration["dndTargetPropertyName"] = extendedMetadata.PropertyName;
		if (extendedMetadata.OverlayConfiguration.TryGetValue("dndTargetPropertyAllowMultiple", out var value3))
		{
			extendedMetadata.Parent.OverlayConfiguration["dndTargetPropertyAllowMultiple"] = value3;
			return;
		}
		Type modelType = extendedMetadata.ModelType;
		if ((modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof(IList<>)) || modelType.GetInterfaces().Any((Type i) => i == typeof(IList)))
		{
			extendedMetadata.Parent.OverlayConfiguration["dndTargetPropertyAllowMultiple"] = true;
		}
	}
}
