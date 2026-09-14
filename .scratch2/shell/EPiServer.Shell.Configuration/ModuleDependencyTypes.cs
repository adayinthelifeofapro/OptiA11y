using System;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Client-client module dependency types
///       </summary>
[Flags]
public enum ModuleDependencyTypes
{
	/// <summary>
	///       Default dependency type, the same to <see cref="F:EPiServer.Shell.Configuration.ModuleDependencyTypes.Require" /></summary>
	None = 0,
	/// <summary>
	///       Run dependant module initializer first
	///       </summary>
	Require = 1,
	/// <summary>
	///       Automatically run current module initializer after the dependant module initializer
	///       </summary>
	RunAfter = 2
}
