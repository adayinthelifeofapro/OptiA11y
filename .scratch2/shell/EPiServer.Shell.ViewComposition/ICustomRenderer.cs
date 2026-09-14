using Microsoft.AspNetCore.Html;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Interface that enables you to take over generation of html for your <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />.
///       </summary>
public interface ICustomRenderer
{
	/// <summary>
	///       Renders the html for the component.
	///       </summary>
	/// <param name="component">The component.</param>
	/// <param name="innerHtml">The inner HTML.</param>
	HtmlString RenderComponent(IComponent component, string innerHtml);
}
