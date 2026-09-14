using System.Collections.Generic;
using EPiServer.ServiceLocation;

namespace EPiServer.Framework.Serialization;

/// <summary>
///       Defines a factory service responsible for creating <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> instances
///       used to serialize objects that should be passed to the UI.
///       </summary>
public interface IObjectSerializerFactory
{
	/// <summary>
	///       Gets all content types that are supported by registered <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /></summary>
	IEnumerable<string> SupportedContentTypes { get; }

	/// <summary>
	///       Gets the <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> that is registered for the provided content type.
	///       </summary>
	/// <param name="contentType">The content type.</param>
	/// <returns>An <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> for the given content type or null if no serializer is registered for the content type.</returns>
	IObjectSerializer GetSerializer(string contentType);

	/// <summary>
	///       Registers an <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> for a given content type.
	///       </summary>
	/// <param name="contentType">The content type to handle.</param>
	/// <param name="accessor">The accessor delegate.</param>
	void RegisterSerializer(string contentType, ServiceAccessor<IObjectSerializer> accessor);
}
