using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Base class for all menu items
///       </summary>
/// <example>
///       This example shows how to create a top level menu called "Dashboard" and sub menu item to it called "Controls".
///       <code>
///       MenuSectionItem topMenu = new MenuSectionItem("Dashboard", "/top/dashboard");
///       RouteMenuItem subMenu = new MenuItem("Controls","/top/dashboard/controls", "Index", new { controller = "ExampleControls", moduleArea="Shell" });
///       </code></example>
public abstract class MenuItem
{
	internal Injected<IHttpContextAccessor> HttpAccessor;

	[CompilerGenerated]
	private string _003CPath_003Ek__BackingField;

	[CompilerGenerated]
	private Func<HttpContext, bool> _003CIsAvailable_003Ek__BackingField = (HttpContext request) => true;

	/// <summary>
	///       Link text
	///       </summary>
	public string Text { get; set; }

	/// <summary>
	///       Tooltip text
	///       </summary>
	public string ToolTip { get; set; }

	/// <summary>
	///       CSS class
	///       </summary>
	public string CssClass { get; set; }

	/// <summary>
	///       CSS Icon class
	///       </summary>
	public string IconName { get; set; }

	/// <summary>
	///       Sort index for the menu item
	///       </summary>
	public int SortIndex { get; set; }

	/// <summary>
	///       The url to the interface referenced by this menu item.
	///       </summary>
	/// <returns>The url for the menu item</returns>
	public string Url { get; set; }

	/// <summary>
	///       The alignment for this menu item in the menu, e.g. Right.
	///       </summary>
	public MenuItemAlignment Alignment { get; set; }

	/// <summary>
	///       The behavior for this menu item in the menu, e.g. FireThenHide.
	///       </summary>
	public MenuItemBehavior Behavior { get; set; }

	/// <summary>
	///       Indicates whether this menu items has any children that should be displayed in the menu.
	///       </summary>
	public virtual bool IsLeaf => true;

	/// <summary>
	///       Will render additional span elements used for adding icons to this menu item via style sheets
	///       </summary>
	public bool IsStyled { get; set; }

	/// <summary>
	///       Unique path for the menu item
	///       </summary>
	/// <example>
	///       /top/forum/search
	///       </example>
	public string Path
	{
		[CompilerGenerated]
		get
		{
			return _003CPath_003Ek__BackingField;
		}
		set
		{
			if (_003CPath_003Ek__BackingField != value)
			{
				_003CPath_003Ek__BackingField = value.EnsureValidMenuPath();
				Depth = _003CPath_003Ek__BackingField.CalculatePathDepth();
			}
		}
	}

	/// <summary>
	///       Target frame.
	///       </summary>
	/// <example>
	///       _blank
	///       </example>
	public string Target { get; set; }

	/// <summary>
	///       Get the depth of the path.
	///       </summary>
	public int Depth { get; private set; }

	/// <summary>
	///       Gets or sets authorization policy name. If set, only users with proper permissions can see this menu item.
	///       </summary>
	public string AuthorizationPolicy { get; set; }

	/// <summary>
	///       The delegate responsible for determining the menuitems avaliability in a certain context. This delegate will not be evaluated if AuthorizationPolicy is set.
	///       </summary>
	public Func<HttpContext, bool> IsAvailable
	{
		[CompilerGenerated]
		get
		{
			return _003CIsAvailable_003Ek__BackingField;
		}
		set
		{
			ArgumentNullException.ThrowIfNull(value, "value");
			_003CIsAvailable_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Gets or sets a value indicating whether this menu item should be excluded from the navigation search provider.
	///       </summary>
	public bool ExcludeFromSearch { get; set; }

	/// <summary>
	///       Gets the css to apply on the li tag that encapsulates the link
	///       </summary>
	protected internal virtual string NodeCssClass
	{
		get
		{
			if (Alignment != MenuItemAlignment.Left)
			{
				return "epi-navigation-util";
			}
			return string.Empty;
		}
	}

	/// <summary>
	///       Get or sets the flag to indicate the menu item is EPi Product or not.
	///       This is automatically set by <see cref="T:EPiServer.Shell.Navigation.Internal.MenuAssembler" />
	///       by detecting the inheritance from <see cref="T:EPiServer.Shell.Navigation.Internal.IEPiProductMenuProvider" /></summary>
	public bool IsEPiMenuItem { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.MenuItem" /> class
	///       </summary>
	/// <param name="text">Text for the link</param>
	/// <param name="path">Unique path for the menu item</param>
	protected MenuItem(string text, string path)
	{
		Path = path;
		Text = text;
		Url = string.Empty;
	}

	/// <summary>
	///       Returns a string with the Path, Text and URL for the <see cref="T:EPiServer.Shell.Navigation.MenuItem" /></summary>
	public override string ToString()
	{
		string name = GetType().Name;
		CultureInfo invariantCulture = CultureInfo.InvariantCulture;
		InlineArray4<object> buffer = default(InlineArray4<object>);
		buffer[0] = Path;
		buffer[1] = Text;
		buffer[2] = Url;
		buffer[3] = SortIndex;
		return name + string.Format((IFormatProvider?)invariantCulture, " {{Path: \"{0}\", Text: \"{1}\", Url: \"{2}\", SortIndex: \"{3}\"}}", (ReadOnlySpan<object?>)buffer);
	}

	/// <summary>
	///       Determines whether the menu item is selected withing the the specified request context.
	///       </summary>
	/// <param name="requestContext">The request context.</param>
	public virtual bool IsSelected(HttpContext requestContext)
	{
		ArgumentNullException.ThrowIfNull(requestContext, "requestContext");
		if (!string.IsNullOrEmpty(Url))
		{
			string value = requestContext.Request?.Path.Value.Trim('/');
			return Url.Trim('/').Equals(value, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	/// <summary>
	///       Renders the menu item contents (probably a link).
	///       </summary>
	/// <param name="writer">The writer.</param>
	protected internal virtual void RenderContents(TextWriter writer)
	{
		writer.WriteFormat("<a href=\"{0}\" class=\"epi-navigation-{1} {2}\"", Url, this.GetClientId(), CssClass);
		writer.WriteFormatUnlessEmpty(" target=\"{0}\"", Target);
		writer.WriteFormatUnlessEmpty(" title=\"{0}\"", ToolTip);
		writer.Write(">");
		if (IsStyled)
		{
			writer.Write("<span class=\"epi-navigation-menuIcon\"><span class=\"epi-navigation-menuArrow\"><span class=\"epi-navigation-menuText\">");
		}
		writer.Write(Text);
		if (IsStyled)
		{
			writer.Write("</span></span></span>");
		}
		writer.Write("</a>");
	}
}
