using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;
using EPiServer.Shell.ObjectEditing.Internal;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Registry that holds all metadataHandlers for entity types.
///       </summary>
public class MetadataHandlerRegistry
{
	private readonly ConcurrentDictionary<string, HandlerRegistrationSettings> _metadataHandlers;

	private readonly IDictionary<string, IModelAccessorCreator> _modelAccessorCreators;

	private readonly IEditorDefinitionRepository _editorDefinitionRepository;

	private readonly ILogger<MetadataHandlerRegistry> _log;

	private readonly Lock _addHandlerLock = new Lock();

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.MetadataHandlerRegistry" /> class.
	///       </summary>
	/// <param name="descriptors">An array of <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.EditorDescriptor" /> objects to register target types for</param>
	/// <param name="modelAccessorCreators">An array of <see cref="T:EPiServer.Shell.ObjectEditing.IModelAccessorCreator" /> objects to register target type for</param>
	/// <param name="editorDefinitionRepository">The repository to use when populating metadata with editor definitions.</param>
	/// <param name="logger">The logger.</param>
	public MetadataHandlerRegistry(IEnumerable<EditorDescriptor> descriptors, IEnumerable<IModelAccessorCreator> modelAccessorCreators, IEditorDefinitionRepository editorDefinitionRepository, ILogger<MetadataHandlerRegistry> logger)
	{
		_metadataHandlers = new ConcurrentDictionary<string, HandlerRegistrationSettings>();
		_modelAccessorCreators = new ConcurrentDictionary<string, IModelAccessorCreator>();
		_editorDefinitionRepository = editorDefinitionRepository;
		_log = logger;
		foreach (EditorDescriptor item in descriptors.Distinct(new EditorDescriptorComparer()))
		{
			foreach (EditorDescriptorRegistrationAttribute customAttribute in ((MemberInfo)item.GetType()).GetCustomAttributes<EditorDescriptorRegistrationAttribute>(false))
			{
				RegisterMetadataHandler(customAttribute.TargetType, item, customAttribute.UIHint, customAttribute.EditorDescriptorBehavior);
			}
		}
		foreach (IModelAccessorCreator modelAccessorCreator in modelAccessorCreators)
		{
			ModelAccessorCreatorRegistrationAttribute modelAccessorCreatorRegistrationAttribute = ((MemberInfo)modelAccessorCreator.GetType()).GetCustomAttributes<ModelAccessorCreatorRegistrationAttribute>(true).FirstOrDefault();
			if (modelAccessorCreatorRegistrationAttribute != null)
			{
				RegisterModelAccessorCreator(modelAccessorCreatorRegistrationAttribute.TargetType, modelAccessorCreator);
			}
		}
	}

	/// <summary>
	///       Gets the metadata handlers.
	///       </summary>
	/// <param name="type">The type.</param>
	/// <returns>A <see cref="T:System.Collections.Generic.IEnumerable`1" /> with the registered handlers.</returns>
	public virtual IEnumerable<IMetadataHandler> GetMetadataHandlers(Type type)
	{
		return GetMetadataHandlers(type, null);
	}

	/// <summary>
	///       Gets all object metadataHandlers for a type.
	///       </summary>
	/// <param name="type">The type.</param>
	/// <param name="uiHint">The type override.</param>
	/// <returns>A <see cref="T:System.Collections.Generic.IEnumerable`1" /> with the registered handlers.</returns>
	public virtual IEnumerable<IMetadataHandler> GetMetadataHandlers(Type type, string uiHint)
	{
		EditorDefinition editorDefinition = _editorDefinitionRepository.Get(type, uiHint);
		if (editorDefinition != null)
		{
			return new EditorDefinitionMetadataExtender[1]
			{
				new EditorDefinitionMetadataExtender(editorDefinition)
			};
		}
		_metadataHandlers.TryGetValue(GetRegistrationKey(type, uiHint), out var value);
		if (value == null)
		{
			return Array.Empty<IMetadataHandler>();
		}
		if (!value.IsInitialized)
		{
			InitializeSettings(type, uiHint, value);
		}
		return value.MetadataHandlers;
	}

	private void InitializeSettings(Type type, string uiHint, HandlerRegistrationSettings settings)
	{
		if (!string.IsNullOrEmpty(uiHint) && settings.MergeBaseDescriptorsOnFirstRequest)
		{
			_metadataHandlers.TryGetValue(GetRegistrationKey(type, null), out var value);
			int num = 0;
			foreach (IMetadataHandler metadataHandler in value.MetadataHandlers)
			{
				settings.MetadataHandlers.Insert(num++, metadataHandler);
			}
			settings.MergeBaseDescriptorsOnFirstRequest = false;
		}
		settings.IsInitialized = true;
	}

	/// <summary>
	///       Registers the metadata handler.
	///       </summary>
	/// <param name="type">The type.</param>
	/// <param name="metadataHandler">The metadata handler.</param>
	public void RegisterMetadataHandler(Type type, IMetadataHandler metadataHandler)
	{
		RegisterMetadataHandler(type, metadataHandler, null, EditorDescriptorBehavior.Default);
	}

	/// <summary>
	///       Registers a metadataHandler for a type.
	///       </summary>
	/// <param name="type">The type.</param>
	/// <param name="metadataHandler">The metadataHandler.</param>
	/// <param name="typeOverride">The type override.</param>
	public void RegisterMetadataHandler(Type type, IMetadataHandler metadataHandler, string typeOverride)
	{
		RegisterMetadataHandler(type, metadataHandler, typeOverride, EditorDescriptorBehavior.Default);
	}

	/// <summary>
	///       Registers a metadataHandler for a type.
	///       </summary>
	/// <param name="type">The type.</param>
	/// <param name="metadataHandler">The metadataHandler.</param>
	/// <param name="uiHint">The type override.</param>
	/// <param name="editorDescriptorBehavior">If the base handlers should be applied before specific handlers.</param>
	public void RegisterMetadataHandler(Type type, IMetadataHandler metadataHandler, string uiHint, EditorDescriptorBehavior editorDescriptorBehavior)
	{
		string registrationKey = GetRegistrationKey(type, uiHint);
		HandlerRegistrationSettings metadataHandlers;
		try
		{
			metadataHandlers = _metadataHandlers.AddOrUpdate(registrationKey, delegate
			{
				metadataHandlers = new HandlerRegistrationSettings();
				return metadataHandlers;
			}, delegate(string _, HandlerRegistrationSettings settings)
			{
				if (settings.BlockNewRegistrations)
				{
					if (editorDescriptorBehavior == EditorDescriptorBehavior.OverrideDefault)
					{
						_log.OverrideDefaultBehaviorWarning(settings.GetType().FullName, settings.MetadataHandlers.First().GetType().FullName);
					}
					throw new ApplicationException();
				}
				return settings;
			});
		}
		catch (ApplicationException)
		{
			return;
		}
		if (!string.IsNullOrEmpty(uiHint) && editorDescriptorBehavior == EditorDescriptorBehavior.ExtendBase)
		{
			metadataHandlers.MergeBaseDescriptorsOnFirstRequest = true;
		}
		else if (editorDescriptorBehavior == EditorDescriptorBehavior.OverrideDefault)
		{
			metadataHandlers.MetadataHandlers.Clear();
			metadataHandlers.BlockNewRegistrations = true;
		}
		using (_addHandlerLock.EnterScope())
		{
			if (editorDescriptorBehavior == EditorDescriptorBehavior.PlaceLast)
			{
				metadataHandlers.MetadataHandlers.Add(metadataHandler);
			}
			else
			{
				metadataHandlers.MetadataHandlers.Insert(0, metadataHandler);
			}
		}
	}

	private static string GetRegistrationKey(Type type, string typeOverride)
	{
		if (!string.IsNullOrEmpty(typeOverride))
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}#{1}", type.FullName, typeOverride.ToLowerInvariant());
		}
		return type.FullName;
	}

	/// <summary>
	///       Registers a metadataHandler for several types.
	///       </summary>
	/// <param name="types">The types.</param>
	/// <param name="metadataHandler">The metadataHandler.</param>
	public void RegisterMetadataHandler(Type[] types, IMetadataHandler metadataHandler)
	{
		foreach (Type type in types)
		{
			RegisterMetadataHandler(type, metadataHandler, null, EditorDescriptorBehavior.Default);
		}
	}

	/// <summary>
	///       Register a model accessor for the given type.
	///       </summary>
	/// <param name="type">The type</param>
	/// <param name="accessorCreator">The accessor creator</param>
	public void RegisterModelAccessorCreator(Type type, IModelAccessorCreator accessorCreator)
	{
		_modelAccessorCreators.Add(type.FullName, accessorCreator);
	}

	/// <summary>
	///       Get model accessor for the given type
	///       </summary>
	/// <param name="type">The type</param>
	/// <param name="arguments">Arguments to be sent to the model accessor creator</param>
	/// <returns>
	/// </returns>
	public Func<object> GetModelAccessor(Type type, Dictionary<string, string> arguments)
	{
		_modelAccessorCreators.TryGetValue(type.FullName, out var value);
		return value?.Create(arguments);
	}
}
