using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace EPiServer.Framework.Serialization;

/// <summary>
///       Extension methods for <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> instances.
///       </summary>
public static class ObjectSerializerExtensions
{
	/// <summary>
	///       Serializes the specified object to a string.
	///       </summary>
	/// <param name="serializer">The serializer.</param>
	/// <param name="value">The object that should be serialized to a string.</param>
	/// <returns>A serialized representation of the provided object</returns>
	public static string Serialize(this IObjectSerializer serializer, object value)
	{
		ArgumentNullException.ThrowIfNull(serializer, "serializer");
		StringBuilder stringBuilder = new StringBuilder(256);
		using (StringWriter textWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
		{
			serializer.Serialize(textWriter, value);
		}
		return stringBuilder.ToString();
	}

	/// <summary>
	///       Deserializes the specified value to an object of type <typeparamref name="T" />.
	///       </summary>
	/// <typeparam name="T">The type that the string should be deserialized to.</typeparam>
	/// <param name="serializer">The serializer.</param>
	/// <param name="value">The value that should be deserialized.</param>
	/// <returns>An object of type T</returns>
	public static T Deserialize<T>(this IObjectSerializer serializer, string value)
	{
		ArgumentNullException.ThrowIfNull(serializer, "serializer");
		ArgumentNullException.ThrowIfNull(value, "value");
		using StringReader reader = new StringReader(value);
		return serializer.Deserialize<T>(reader);
	}

	public static object Deserialize(this IObjectSerializer serializer, string value, Type objectType)
	{
		ArgumentNullException.ThrowIfNull(serializer, "serializer");
		ArgumentNullException.ThrowIfNull(value, "value");
		using StringReader reader = new StringReader(value);
		return serializer.Deserialize(reader, objectType);
	}
}
