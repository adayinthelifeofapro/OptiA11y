using System;
using System.Collections.Generic;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Implement this interface to be able to provide metadata for an entire class and it's sub properties.
///       </summary>
public interface IMetadataProvider : IMetadataHandler
{
	/// <summary>
	///       Gets the metadata for the specified property.
	///       </summary>
	/// <param name="attributes">The attributes.</param>
	/// <param name="containerType">The type of the container.</param>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <param name="modelType">The type of the model.</param>
	/// <param name="propertyName">The name of the property.</param>
	/// <returns>The metadata for the property.</returns>
	ExtendedMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName);

	/// <summary>
	///       Gets the metadata for the properties of a given type.
	///       </summary>
	/// <param name="container">The container.</param>
	/// <param name="containerType">Type of the container.</param>
	/// <returns>The metadata for the properties of the type.</returns>
	IEnumerable<ExtendedMetadata> GetMetadataForProperties(object container, Type containerType);
}
