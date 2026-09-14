using System.Collections.Concurrent;
using System.Collections.Generic;
using EPiServer.Extensions.Internal;
using EPiServer.ServiceLocation;

namespace EPiServer.Framework.Serialization.Internal;

/// <internal-API />
public class ObjectSerializerFactory : IObjectSerializerFactory
{
	private readonly ConcurrentDictionary<string, ServiceAccessor<IObjectSerializer>> _serializers;

	/// <inheritdoc />
	public IEnumerable<string> SupportedContentTypes => _serializers.Keys;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Framework.Serialization.Internal.ObjectSerializerFactory" /> class.
	///       </summary>
	public ObjectSerializerFactory(IEnumerable<ServiceAccessor<IObjectSerializer>> serializers)
	{
		_serializers = new ConcurrentDictionary<string, ServiceAccessor<IObjectSerializer>>();
		foreach (ServiceAccessor<IObjectSerializer> serializer in serializers)
		{
			RegisterSerializer(serializer);
		}
	}

	/// <summary>
	///       Registers an <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> for a given content type.
	///       </summary>
	/// <param name="serializer">The serializer.</param>
	/// <remarks>This overrides any existing serializer.</remarks>
	public void RegisterSerializer(IObjectSerializer serializer)
	{
		RegisterSerializer(() => serializer);
	}

	/// <summary>
	///       Registers an <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> for a given content type.
	///       </summary>
	/// <param name="contentType">The content type to handle.</param>
	/// <param name="serializer">The serializer.</param>
	/// <remarks>This overrides any existing serializer.</remarks>
	public void RegisterSerializer(string contentType, IObjectSerializer serializer)
	{
		RegisterSerializer(contentType, () => serializer);
	}

	/// <summary>
	///       Registers an <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> for a given content type.
	///       </summary>
	/// <param name="contentType">The content type to handle.</param>
	/// <param name="accessor">The creator function.</param>
	public void RegisterSerializer(string contentType, ServiceAccessor<IObjectSerializer> accessor)
	{
		_serializers[contentType] = accessor;
	}

	/// <summary>
	///       Registers an <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> for all content types that the serializer specifies in <see cref="P:EPiServer.Framework.Serialization.IObjectSerializer.HandledContentTypes" />.
	///       </summary>
	/// <param name="accessor">The accessor.</param>
	/// <remarks>
	///       This replaces any existing serializer.
	///       </remarks>
	public void RegisterSerializer(ServiceAccessor<IObjectSerializer> accessor)
	{
		IObjectSerializer objectSerializer = accessor.Invoke();
		ArgumentExceptionExtensions.ThrowIfPropertyIsNull(objectSerializer.HandledContentTypes, null, null, "serializer.HandledContentTypes");
		foreach (string handledContentType in objectSerializer.HandledContentTypes)
		{
			RegisterSerializer(handledContentType, accessor);
		}
	}

	/// <summary>
	///       Gets the serializer for the given content type.
	///       </summary>
	/// <param name="contentType">The content type.</param>
	/// <returns>
	///       An <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> for the given content type or null if no serializer is registered for the content type.
	///       </returns>
	public virtual IObjectSerializer GetSerializer(string contentType)
	{
		if (!_serializers.TryGetValue(contentType, out var value))
		{
			return null;
		}
		return value.Invoke();
	}
}
