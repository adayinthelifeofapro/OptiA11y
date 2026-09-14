using System;
using System.IO;
using EPiServer.Framework.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Uses an <see cref="T:EPiServer.Framework.Serialization.IObjectSerializerFactory" /> to get a serializer that can deserialize the post data.
///       </summary>
/// <remarks>Currently only supports the JSON format.</remarks>
public class RestPostDataProvider : IRestControllerValueProvider
{
	private readonly IObjectSerializerFactory _serializerFactory;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RestPostDataProvider" /> class.
	///       </summary>
	/// <param name="serializerFactory">The serializer.</param>
	public RestPostDataProvider(IObjectSerializerFactory serializerFactory)
	{
		ArgumentNullException.ThrowIfNull(serializerFactory, "serializerFactory");
		_serializerFactory = serializerFactory;
	}

	/// <summary>
	///       Returns the post data if the requested parameter name is <c>entity</c>.
	///       </summary>
	/// <param name="controllerContext">The controller context.</param>
	/// <param name="parameterDescriptor">The parameter descriptor.</param>
	/// <returns>
	///       The parameter value if a match was made; otherwise false.
	///       </returns>
	public object GetParameterValue(ActionContext controllerContext, ParameterDescriptor parameterDescriptor)
	{
		if (string.Equals("entity", parameterDescriptor.Name, StringComparison.OrdinalIgnoreCase))
		{
			return GetDeserializedPostData(controllerContext, parameterDescriptor);
		}
		return null;
	}

	/// <summary>
	///       Gets the post data deserialized into an object using JSON serializer.
	///       </summary>
	/// <param name="controllerContext">The controller context.</param>
	/// <param name="descriptor">The descriptor.</param>
	/// <returns>An object if the request contains json post data.</returns>
	private object GetDeserializedPostData(ActionContext controllerContext, ParameterDescriptor descriptor)
	{
		if (controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
		{
			IObjectSerializer obj = _serializerFactory.GetSerializer("application/json") ?? throw new InvalidOperationException("No serializer registered for content type: application/json");
			StreamReader reader = new StreamReader(controllerContext.HttpContext.Request.Body);
			return obj.Deserialize(reader, descriptor.ParameterType);
		}
		return null;
	}
}
