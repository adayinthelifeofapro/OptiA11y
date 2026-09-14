using System;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Used to forbid or unavailable access when modules are stored in the "_Protected" folder and the application is in the ReadOnly mode.
///       </summary>
public interface IAccessReadOnlyProtectedModules
{
	/// <summary>
	///       Occurs when database mode is readonly and access a protected module
	///       </summary>
	event EventHandler<ReadOnlyProtectedModuleEventArgs> AccessPath;
}
