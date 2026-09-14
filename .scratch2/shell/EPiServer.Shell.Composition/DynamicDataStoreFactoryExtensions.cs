using System;
using EPiServer.Data.Dynamic;

namespace EPiServer.Shell.Composition;

/// <summary>
///       Extension methods for dynamic data store.
///       </summary>
public static class DynamicDataStoreFactoryExtensions
{
	/// <summary>
	///       Gets or creates the dynamic data store for the supplied type.
	///       </summary>
	/// <param name="factory">The factory.</param>
	/// <param name="storedObjectType">Type of the stored object.</param>
	/// <returns>
	/// </returns>
	public static DynamicDataStore GetOrCreateStore(this DynamicDataStoreFactory factory, Type storedObjectType)
	{
		return factory.GetStore(storedObjectType) ?? factory.CreateStore(storedObjectType);
	}
}
