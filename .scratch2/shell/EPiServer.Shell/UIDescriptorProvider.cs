using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell;

/// <summary>
///       Base class for providing <see cref="T:EPiServer.Shell.UIDescriptor" />s to the <see cref="T:EPiServer.Shell.UIDescriptorRegistry" /></summary>
public abstract class UIDescriptorProvider
{
	/// <summary>
	///       Implement this method to provide custom ui descriptors
	///       </summary>
	/// <returns>A collection of ui descriptors</returns>
	public abstract IEnumerable<UIDescriptor> GetDescriptors();

	/// <summary>
	///       Gets a token that is signaled when the set of descriptors has changed.
	///       The default implementation returns a token that is never signaled, meaning that the descriptors from the provider are static and will not change.
	///       </summary>
	public virtual IChangeToken GetChangeToken()
	{
		return NullChangeToken.Singleton;
	}
}
