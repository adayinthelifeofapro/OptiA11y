using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EPiServer.Framework.Serialization;

namespace EPiServer.Shell.Serialization.Json.Internal;

/// <summary>
///       An <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> implementation that uses registered IObjectSerializers for serailization depending on type or assembly
///       </summary>
public class CompositeJsonObjectSerializer : IObjectSerializer
{
	private IObjectSerializer _fallbackSerializer;

	private readonly Dictionary<Assembly, IObjectSerializer> _assemblySerializers = new Dictionary<Assembly, IObjectSerializer>();

	private readonly Dictionary<Type, IObjectSerializer> _typeSerializers = new Dictionary<Type, IObjectSerializer>();

	private static readonly IEnumerable<string> _handledContentTypes = new List<string> { "application/json" }.AsReadOnly();

	/// <inheritdoc />
	public IEnumerable<string> HandledContentTypes => _handledContentTypes;

	/// <summary>
	///       Creates a new instance of <see cref="T:EPiServer.Shell.Serialization.Json.Internal.CompositeJsonObjectSerializer" /></summary>
	public CompositeJsonObjectSerializer(IObjectSerializer fallbackSerializer)
	{
		_fallbackSerializer = fallbackSerializer;
	}

	/// <summary>
	///       Register an <see cref="T:EPiServer.Framework.Serialization.IObjectSerializer" /> as a fallback serializer when no assembly or type registration apply
	///       </summary>
	public void RegisterFallbackSerializer(IObjectSerializer objectSerializer)
	{
		_fallbackSerializer = objectSerializer;
	}

	/// <summary>
	///       Registers an object serializer for an assembly
	///       </summary>
	public void Register(Assembly assembly, IObjectSerializer objectSerializer)
	{
		_assemblySerializers[assembly] = objectSerializer;
	}

	/// <summary>
	///       Registers an object serializer for a type
	///       </summary>
	public void Register(Type type, IObjectSerializer objectSerializer)
	{
		_typeSerializers[type] = objectSerializer;
	}

	/// <inheritdoc />
	public T Deserialize<T>(TextReader reader)
	{
		return (T)Deserialize(reader, typeof(T));
	}

	/// <inheritdoc />
	public object Deserialize(TextReader reader, Type objectType)
	{
		return Resolve(objectType).Deserialize(reader, objectType);
	}

	/// <inheritdoc />
	public void Serialize(TextWriter textWriter, object value)
	{
		if (value != null)
		{
			Resolve(value.GetType()).Serialize(textWriter, value);
		}
	}

	internal IObjectSerializer Resolve(Type type)
	{
		type = GetEnumerableTypeArgumentOrType(type);
		if (!_typeSerializers.TryGetValue(type, out var value) && !_assemblySerializers.TryGetValue(type?.Assembly, out value))
		{
			return _fallbackSerializer;
		}
		return value;
	}

	private static Type GetEnumerableTypeArgumentOrType(Type type)
	{
		if (type.IsInterface && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
		{
			return type.GetGenericArguments()[0];
		}
		Type[] interfaces = type.GetInterfaces();
		foreach (Type type2 in interfaces)
		{
			if (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(IEnumerable<>))
			{
				return type2.GetGenericArguments()[0];
			}
		}
		return type;
	}
}
