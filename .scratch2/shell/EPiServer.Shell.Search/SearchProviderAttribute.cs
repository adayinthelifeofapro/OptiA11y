using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Search;

/// <summary>
///       Search provider attribute. A wrapper for the MEF's Export attribute for ISearchProvider interface.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class SearchProviderAttribute : ServicePlugInAttributeBase
{
	/// <summary>
	///       Gets <see cref="F:EPiServer.ServiceLocation.ServiceInstanceScope.Singleton" />.
	///       </summary>
	public override ServiceInstanceScope Lifecycle => (ServiceInstanceScope)0;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Search.SearchProviderAttribute" /> class.
	///       </summary>
	public SearchProviderAttribute()
		: base(typeof(ISearchProvider))
	{
	}
}
