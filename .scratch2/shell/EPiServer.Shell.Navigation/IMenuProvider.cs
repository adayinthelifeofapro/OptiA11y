using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Classes implementing this interface can be used to provide links to the
///       shell menu.
///       </summary>
/// <remarks>
///       Besides implementing the interface menu providers must also be registered in the IoC container
///       Please Implement GetMenuItemsAsync() as it is the method used by the framework.
///       </remarks>
public interface IMenuProvider
{
	/// <summary>
	///       This method is called when the menu is being assembled.
	///       </summary>
	/// <returns>A list of <see cref="T:EPiServer.Shell.Navigation.MenuItem" />s that the provider exposes.</returns>
	/// <remarks>
	///       The Provider implementation has to handle the security themselves.
	///       </remarks>
	IEnumerable<MenuItem> GetMenuItems()
	{
		return Array.Empty<MenuItem>();
	}

	/// <summary>
	///       Gets the menu items asynchronously.
	///       </summary>
	/// <returns>A Task that represents the asynchronous operation, containing a list of <see cref="T:EPiServer.Shell.Navigation.MenuItem" />s that the provider exposes.</returns>
	/// <remarks>
	///       The Provider implementation has to handle the security themselves.
	///       </remarks>
	IAsyncEnumerable<MenuItem> GetMenuItemsAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		return GetMenuItems().ToAsyncEnumerable();
	}
}
