using System;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Should be implemented if you want to receive event notifications.
///       </summary>
public interface IComponentNotify
{
	/// <summary>
	///       Will be called when the component is deleted.
	///       </summary>
	/// <param name="id">The id of the component.</param>
	void OnDeleted(Guid id);
}
