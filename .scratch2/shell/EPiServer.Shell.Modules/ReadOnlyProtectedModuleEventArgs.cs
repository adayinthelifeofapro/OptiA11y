using System;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Event argument used when access protected module and the database is in the ReadOnly mode.
///       </summary>
public class ReadOnlyProtectedModuleEventArgs : EventArgs
{
	/// <summary>
	///       The requested path. 
	///       </summary>
	public string RequestedPath { get; }

	/// <summary>
	///       Indicates if the event has been handled.
	///       </summary>
	public bool Handled { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ReadOnlyProtectedModuleEventArgs" /> class.
	///       </summary>
	/// <param name="requstedPath">The requested path.</param>
	public ReadOnlyProtectedModuleEventArgs(string requstedPath)
	{
		RequestedPath = requstedPath;
	}
}
