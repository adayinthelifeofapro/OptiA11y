using System;
using System.Collections.Generic;
using System.IO;

namespace EPiServer.Framework.Serialization;

/// <summary>
///       Defines a service responsible for serializing objects so they can be passed to and from the UI.
///       </summary>
/// <remarks>
///       This serializer is exclusively intended to be used when serializing objects for the Episerver
///       user interface. Usage beyond this scope can have unintended side effects and is not supported.
///       </remarks>
public interface IObjectSerializer
{
	/// <summary>
	///       Gets a list of all content types that are handled by this <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" />.
	///       </summary>
	IEnumerable<string> HandledContentTypes { get; }

	/// <summary>
	///       Serializes an object to be passed to the UI using the specified text writer.
	///       </summary>
	/// <param name="textWriter">The text writer to where the serialized data is written.</param>
	/// <param name="value">The object that should be serialized.</param>
	void Serialize(TextWriter textWriter, object value);

	/// <summary>
	///       Deserializes a string passed from the UI using the specified reader.
	///       </summary>
	/// <param name="reader">A reader provided the serialized data.</param>
	/// <param name="objectType">The type of the object to deserialize.</param>
	/// <returns>The deserialized object.</returns>
	object Deserialize(TextReader reader, Type objectType);

	/// <summary>
	///       Deserializes a string passed from the UI using the specified reader.
	///       </summary>
	/// <typeparam name="T">The type of object to deserialize.</typeparam>
	/// <param name="reader">A reader provided the serialized data.</param>
	/// <returns>The deserialized object.</returns>
	T Deserialize<T>(TextReader reader);
}
