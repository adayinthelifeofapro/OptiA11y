using System;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Gets a controller that will be used when a module dosn't have a controller.
///       </summary>
public interface IFallbackModuleController
{
	/// <summary>
	///       The type of the controller that will be used when a module have a view without a controller.
	///       </summary>
	Type ControllerType { get; }
}
