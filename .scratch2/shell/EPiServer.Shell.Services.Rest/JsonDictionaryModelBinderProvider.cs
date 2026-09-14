using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Provides an <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionaryModelBinder" /> for models of type <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionary" />.
///       </summary>
public class JsonDictionaryModelBinderProvider : IModelBinderProvider
{
	private readonly IServiceProvider _serviceProvider;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionaryModelBinderProvider" /> class.
	///       </summary>
	/// <param name="serviceProvider">The service locator to use to create the model binder.</param>
	public JsonDictionaryModelBinderProvider(IServiceProvider serviceProvider)
	{
		ArgumentNullException.ThrowIfNull(serviceProvider, "serviceProvider");
		_serviceProvider = serviceProvider;
	}

	/// <summary>
	///       Gets an instance of a <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionaryModelBinder" /> if the requested type inherits from <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionary" />.
	///       </summary>
	/// <param name="modelType">Type of the requested model.</param>
	/// <returns>An instance of <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionaryModelBinder" /> if the model type inherits <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionary" />; otherwise null.</returns>
	public IModelBinder GetBinder(Type modelType)
	{
		if (typeof(JsonDictionary).IsAssignableFrom(modelType))
		{
			return _serviceProvider.GetService<JsonDictionaryModelBinder>();
		}
		return null;
	}

	public IModelBinder GetBinder(ModelBinderProviderContext context)
	{
		return GetBinder(context.Metadata.ModelType);
	}
}
