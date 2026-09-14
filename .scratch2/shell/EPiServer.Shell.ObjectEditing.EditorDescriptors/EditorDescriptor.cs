using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Used for holding registered client side editors in the <see cref="T:EPiServer.Shell.ObjectEditing.MetadataHandlerRegistry" /></summary>
public class EditorDescriptor : IMetadataExtender, IMetadataHandler
{
	[CompilerGenerated]
	private Type _003CSelectionFactoryType_003Ek__BackingField;

	/// <summary>
	///       Gets or sets the allowed types.
	///       </summary>
	/// <value>
	///       The allowed types.
	///       </value>
	/// <remarks>This is used for setting dnd types to determine what can be dropped on an editor or overlay.</remarks>
	public virtual IEnumerable<Type> AllowedTypes { get; set; }

	/// <summary>
	///       Gets or sets the specific format suffix if any. For instance "reference" or "light" for different content formats.
	///       </summary>
	/// <value>
	///       The allowed types format suffix.
	///       </value>
	public string AllowedTypesFormatSuffix { get; set; }

	/// <summary>
	///       Gets or sets the name of the inner property to extract data from in an drag and drop operation.
	///       </summary>
	/// <value>
	///       The name of the inner property to extract for drag and drop operations.
	///       </value>
	/// <remarks>For instance, you can set url to extract the url property from a link object.</remarks>
	public string DndSourcePropertyName { get; set; }

	/// <summary>
	///       Gets or sets the name of the child property that should accept drag and drop operations to the overlay.
	///       </summary>
	/// <value>
	///       The name of the child property that can accept drag and drop for the overlay.
	///       </value>
	public string DndTargetPropertyName { get; set; }

	/// <summary>
	///       Gets or sets the client editing class, usually a DOJO widget.
	///       </summary>
	/// <value>The client editing class.</value>
	public string ClientEditingClass { get; set; }

	/// <summary>
	///       Gets or sets the client editing class package, which needs to be required, usually a dojo package.
	///       Leave it empty if package name is the same to class name.
	///       </summary>
	/// <value>
	///       The client editing class package.
	///       </value>
	public string ClientEditingPackage { get; set; }

	/// <summary>
	///       Gets or sets the layout class.
	///       </summary>
	/// <value>
	///       The layout class.
	///       </value>
	public string LayoutClass { get; set; }

	/// <summary>
	///       Gets or sets the default binding value.
	///       </summary>
	/// <value>
	///       The default binding value.
	///       </value>
	public virtual object DefaultValue { get; set; }

	/// <summary>
	///       Gets or sets the editor configuration. This will be passed to the editor class's constructor
	///       </summary>
	/// <value>
	///       The editor configuration.
	///       </value>
	public IDictionary<string, object> EditorConfiguration { get; private set; }

	/// <summary>
	///       Gets or sets the configuration data for the overlay. This will be passed to the overlay class's contructor
	///       </summary>
	/// <value>
	///       The overlay configuraion.
	///       </value>
	public IDictionary<string, object> OverlayConfiguration { get; private set; }

	/// <summary>
	///       Gets or sets the type of the selection factory.
	///       </summary>
	/// <value>
	///       The type of the selection factory.
	///       </value>
	public Type SelectionFactoryType
	{
		[CompilerGenerated]
		get
		{
			return _003CSelectionFactoryType_003Ek__BackingField;
		}
		set
		{
			if (value != null && !typeof(ISelectionFactory).IsAssignableFrom(value))
			{
				throw new InvalidCastException("Selection factory type must be a class which implement ISelectionFactory");
			}
			_003CSelectionFactoryType_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.EditorDescriptor" /> class.
	///       </summary>
	public EditorDescriptor()
	{
		OverlayConfiguration = new Dictionary<string, object>();
		EditorConfiguration = new Dictionary<string, object>();
	}

	/// <summary>
	///       Modifies the metadata, adding any custom data the client needs.
	///       </summary>
	/// <remarks>
	///       This method should only be overriden when you need the entire metedata object to work with.
	///       Otherwise, metadata properties should be set by setting the corresponding properties in the editor concrete descriptors' constructor.
	///       Also be aware that modifying metadata object will overwrite all data annotation attributes used in model class.
	///       </remarks>
	/// <param name="metadata">The metadata.</param>
	/// <param name="attributes">The custom attributes attached to the model class</param>
	public virtual void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
	{
		ExtendedMetadata extendedMetadata = metadata;
		if (extendedMetadata.ClientEditingClass == null)
		{
			string text = (extendedMetadata.ClientEditingClass = ClientEditingClass);
		}
		extendedMetadata = metadata;
		if (extendedMetadata.ClientEditingPackage == null)
		{
			string text = (extendedMetadata.ClientEditingPackage = ClientEditingPackage);
		}
		extendedMetadata = metadata;
		if (extendedMetadata.LayoutClass == null)
		{
			string text = (extendedMetadata.LayoutClass = LayoutClass);
		}
		extendedMetadata = metadata;
		if (extendedMetadata.InitialValue == null)
		{
			object obj = (extendedMetadata.InitialValue = (IsNullable(metadata.ModelType) ? null : Activator.CreateInstance(metadata.ModelType)));
		}
		SetEditorConfiguration(metadata);
		SetOverlayConfiguration(metadata);
		extendedMetadata = metadata;
		if ((object)extendedMetadata.SelectionFactoryType == null)
		{
			Type type = (extendedMetadata.SelectionFactoryType = SelectionFactoryType);
		}
		if (AllowedTypes != null)
		{
			AssignAllowedTypesSettings(metadata);
		}
	}

	/// <summary>
	///       Async version of <see cref="M:EPiServer.Shell.ObjectEditing.EditorDescriptors.EditorDescriptor.ModifyMetadata(EPiServer.Shell.ObjectEditing.ExtendedMetadata,System.Collections.Generic.IEnumerable{System.Attribute})" />. Override this in subclasses that need async operations.
	///       The default implementation calls <see cref="M:EPiServer.Shell.ObjectEditing.EditorDescriptors.EditorDescriptor.ModifyMetadata(EPiServer.Shell.ObjectEditing.ExtendedMetadata,System.Collections.Generic.IEnumerable{System.Attribute})" />.
	///       </summary>
	public virtual Task ModifyMetadataAsync(ExtendedMetadata metadata, IEnumerable<Attribute> attributes, CancellationToken cancellationToken = default(CancellationToken))
	{
		ModifyMetadata(metadata, attributes);
		return Task.CompletedTask;
	}

	private void AssignAllowedTypesSettings(ExtendedMetadata metadata)
	{
		IEnumerable<string> enumerable = AllowedTypes.Select((Type a) => a.FullName.ToLowerInvariant());
		metadata.EditorConfiguration["AllowedTypes"] = enumerable;
		if (!string.IsNullOrEmpty(AllowedTypesFormatSuffix))
		{
			metadata.AdditionalValues["TypesFormatSuffix"] = AllowedTypesFormatSuffix;
			enumerable = enumerable.Select((string a) => a + "." + AllowedTypesFormatSuffix);
		}
		metadata.EditorConfiguration["AllowedDndTypes"] = enumerable;
		metadata.OverlayConfiguration["AllowedDndTypes"] = enumerable;
		if (!string.IsNullOrEmpty(DndSourcePropertyName))
		{
			metadata.EditorConfiguration["dndSourcePropertyName"] = DndSourcePropertyName;
			metadata.OverlayConfiguration["dndSourcePropertyName"] = DndSourcePropertyName;
		}
		if (!string.IsNullOrEmpty(DndTargetPropertyName))
		{
			metadata.EditorConfiguration["dndTargetPropertyName"] = DndTargetPropertyName;
			metadata.OverlayConfiguration["dndTargetPropertyName"] = DndTargetPropertyName;
		}
	}

	/// <summary>
	///       Sets the initial configuration for the widget.
	///       </summary>
	/// <param name="metadata">The metadata.</param>
	protected virtual void SetEditorConfiguration(ExtendedMetadata metadata)
	{
		if (EditorConfiguration == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in EditorConfiguration)
		{
			metadata.EditorConfiguration[item.Key] = item.Value;
		}
	}

	/// <summary>
	///       Sets the initial configuration for the widget.
	///       </summary>
	/// <param name="metadata">The metadata.</param>
	protected virtual void SetOverlayConfiguration(ExtendedMetadata metadata)
	{
		if (OverlayConfiguration == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> item in OverlayConfiguration)
		{
			metadata.OverlayConfiguration[item.Key] = item.Value;
		}
	}

	private static bool IsNullable(Type type)
	{
		if (type == null)
		{
			return true;
		}
		if (!type.IsValueType)
		{
			return true;
		}
		if (Nullable.GetUnderlyingType(type) != null)
		{
			return true;
		}
		return false;
	}
}
