using System;
using System.IO;
using System.Threading.Tasks;
using EPiServer.Framework.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       A model binder for JSON formatted dictionaries.
///       </summary>
public class JsonDictionaryModelBinder : IModelBinder
{
	private readonly IObjectSerializerFactory _serializerFactory;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionaryModelBinder" /> class.
	///       </summary>
	/// <param name="serializerFactory">The object serializer factory to use to create a JSON serializer.</param>
	public JsonDictionaryModelBinder(IObjectSerializerFactory serializerFactory)
	{
		_serializerFactory = serializerFactory;
	}

	/// <summary>
	///       Binds the model to a value by using the specified controller context and binding context.
	///       </summary>
	/// <returns>The bound value as a <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionary" />.</returns>
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext, "bindingContext");
		ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
		if (string.IsNullOrEmpty(value.FirstValue))
		{
			bindingContext.Result = ModelBindingResult.Failed();
			return Task.CompletedTask;
		}
		IObjectSerializer serializer = _serializerFactory.GetSerializer("application/json");
		JsonDictionary model;
		using (StringReader reader = new StringReader(value.FirstValue))
		{
			model = serializer.Deserialize<JsonDictionary>(reader);
		}
		bindingContext.Result = ModelBindingResult.Success(model);
		return Task.CompletedTask;
	}
}
