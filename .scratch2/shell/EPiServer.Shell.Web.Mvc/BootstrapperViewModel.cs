using System.Collections.Generic;
using EPiServer.Shell.Modules;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Used to start a shell client side view
///       </summary>
public class BootstrapperViewModel
{
	/// <summary>
	///       Title of the loaded view
	///       </summary>
	public string ViewTitle { get; set; }

	/// <summary>
	///       The name of the view to load.
	///       </summary>
	public string ViewName { get; set; }

	/// <summary>
	///       All modules available in the system.
	///       </summary>
	/// <value>The modules.</value>
	public IEnumerable<ModuleViewModel> Modules { get; set; }

	/// <summary>
	///       Gets or sets the name of the shell module to start.
	///       </summary>
	/// <value>
	///       The name of the shell module.
	///       </value>
	public string ModuleName { get; set; }

	/// <summary>
	///       Defines a default context for the view, for instance the start page for the CMS home view.
	///       </summary>
	/// <remarks>Set to null if the view should not have a default context.</remarks>
	public string DefaultContext
	{
		get
		{
			return ViewSettings["DefaultContext"] as string;
		}
		set
		{
			ViewSettings["DefaultContext"] = value;
		}
	}

	/// <summary>
	///       Gets or sets the component categories that filters what component can be added to the view.
	///       </summary>
	/// <value>
	///       The component categories.
	///       </value>
	public IEnumerable<string> ComponentCategories
	{
		get
		{
			return ViewSettings["ComponentCategories"] as IEnumerable<string>;
		}
		set
		{
			ViewSettings["ComponentCategories"] = value;
		}
	}

	/// <summary>
	///       Gets or sets the view settings.
	///       </summary>
	/// <value>
	///       The view settings.
	///       </value>
	public Dictionary<string, object> ViewSettings { get; private set; }

	/// <summary>
	///       When true, then Platform Navigation should be rendered
	///       </summary>
	public bool RenderNavigation { get; set; } = true;

	/// <summary>
	///       relative or absolute URL to the custom stylesheet
	///       </summary>
	public string CustomStylesheetUrl { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Mvc.BootstrapperViewModel" /> class.
	///       </summary>
	public BootstrapperViewModel()
	{
		ViewSettings = new Dictionary<string, object>();
	}
}
