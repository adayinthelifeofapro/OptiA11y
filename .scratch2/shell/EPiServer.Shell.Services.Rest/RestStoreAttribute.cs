using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Added to <see cref="T:EPiServer.Shell.Services.Rest.RestControllerBase" /> in order to register them as rest controllers.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RestStoreAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       The store name is used as url segment when accessing this store.
	///       </summary>
	public string StoreName { get; private set; }

	/// <summary>
	///       Creates an instance of <see cref="T:EPiServer.Shell.Services.Rest.RestStoreAttribute" /> with store name.
	///       </summary>
	/// <param name="storeName">The store name.</param>
	public RestStoreAttribute(string storeName)
		: base((Type)null)
	{
		StoreName = storeName;
	}
}
