using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Provides bootstrapper view name
///       </summary>
public interface IBootstrapper
{
	/// <summary>
	///       Gets the name of the bootstrapper view.
	///       </summary>
	/// <value>
	///       The name of the bootstrapper view.
	///       </value>
	string BootstrapperViewName { get; }

	/// <summary>
	///       Creates the view model.
	///       </summary>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="context">The context.</param>
	/// <param name="moduleName">The name of the shell module to startup</param>
	/// <returns>
	/// </returns>
	BootstrapperViewModel CreateViewModel(string viewName, ControllerContext context, string moduleName);
}
