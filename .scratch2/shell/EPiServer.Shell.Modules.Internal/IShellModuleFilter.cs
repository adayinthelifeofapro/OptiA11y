namespace EPiServer.Shell.Modules.Internal;

/// <summary>
///       When ShellModule implementing <see cref="T:EPiServer.Shell.Modules.Internal.IShellModuleFilter" /> it can be filtered from Edit Mode
///       </summary>
public interface IShellModuleFilter
{
	/// <summary>
	///       When true, then module is used in edit mode
	///       </summary>
	bool IsModuleEnabled(ShellModule shellModule);
}
