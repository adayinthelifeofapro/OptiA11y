using System;
using EPiServer.ServiceLocation;

namespace EPiServer.Web;

/// <summary>
///       Contains options for Episerver user interface
///       </summary>
[Options(ConfigurationSection = "CmsUI")]
public class UIOptions
{
	/// <summary>
	///       Gets or sets the URL to the Util part of the UI application. It must be a Web URL, and may include scheme and port.
	///       It must be a Web URL, and include the path to the site root. It is only to be used
	///       to generate direct references to the site in external locations. For references from a page, use root-relative
	///       URLs and ResolveUrl etc as appropriate.
	///       </summary>
	/// <remarks>
	///       Default value is '~/Util/'
	///       </remarks>
	public Uri UtilUrl { get; set; } = new Uri("~/Util/", UriKind.Relative);

	/// <summary>
	///       Gets or sets the URL to the edit UI. It must be a Web URL, and may include scheme and port.
	///       It must be a Web URL, and include the path to the site root. It is only to be used
	///       to generate direct references to the site in external locations. For references from a page, use root-relative
	///       URLs and ResolveUrl etc as appropriate.
	///       </summary>
	/// <remarks>
	///       Default value is '~/Optimizely/CMS/'
	///       </remarks>
	public Uri EditUrl { get; set; } = new Uri("~/Optimizely/CMS/", UriKind.Relative);

	/// <summary>
	///       Defines the amount of user roles which would trigger a warning when an administrator sets up approval step
	///       </summary>
	public int ApprovalStepRoleUserLimit { get; set; } = 100;

	/// <summary>
	///       Defines if the globalization module should be used
	///       </summary>
	/// <remarks>
	///       Default value is true
	///       </remarks>
	public bool UIShowGlobalizationUserInterface { get; set; } = true;

	/// <summary>
	///       Defines if the WebSocket connection between server and the client should be enabled
	///       </summary>
	public bool WebSocketEnabled { get; set; } = true;

	/// <summary>
	///       Gets or sets the retain period for the page's "permanent edit" status.
	///       </summary>
	/// <value>The retain period.</value>
	public TimeSpan PermanentEditRetainPeriod { get; set; } = TimeSpan.FromDays(30);

	/// <summary>
	///       Defines the default time to load a preview
	///       </summary>
	/// <remarks>
	///       Default value is 15 seconds
	///       </remarks>
	public int PreviewTimeout { get; set; } = 15000;

	/// <summary>
	///       Gets or sets a value indicating whether version deletion is disabled.
	///       </summary>
	/// <value>
	///   <c>true</c> if version deletion is disabled; otherwise, <c>false</c>.
	///       </value>
	/// <remarks>
	///       Default value false.
	///       </remarks>
	public bool DisableVersionDeletion { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether media files should be published automatically when uploaded to the server
	///       </summary>
	/// <value>
	///   <c>true</c> if media published automatically when uploaded to the server; otherwise, <c>false</c>.
	///       </value>
	/// <remarks>
	///       Default value true.
	///       </remarks>
	public bool AutoPublishMediaOnUpload { get; set; } = true;

	/// <summary>
	///       Gets or sets the the theme class(es) used for the body element.
	///       </summary>
	public string ThemeClass { get; set; } = "Sleek";

	/// <summary>
	///       Set the ReadOnlyInfoUrl option to override the default ReadOnly page (~/Util/ReadOnly)
	///       </summary>
	public string ReadOnlyInfoUrl { get; set; }

	/// <summary>
	///       Defines what the default state the "Update modified date" check box should be in when editing a page.
	///       </summary>
	/// <remarks>
	///       Default value false.
	///       </remarks>
	public bool UIDefaultValueForSetChangedOnPublish { get; set; }

	/// <summary>
	///       Defines if it is possible to create an inline blockdata inside ContentArea properties
	///       </summary>
	/// <remarks>
	///       Default value true.
	///       </remarks>
	public bool InlineBlocksInContentAreaEnabled { get; set; } = true;

	/// <summary>
	///       Defines if property types list should be simplified. Legacy property types should be hidden and property types should be groupped
	///       </summary>
	public bool SimplifyPropertyList { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether preview token should be appended to the preview url
	///       </summary>
	public bool UsePreviewTokens { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether the content asset folder
	///       should be deleted/restored when the content is deleted/restored.
	///       </summary>
	public bool MoveContentAssetFolderWithContentOwner { get; set; } = true;
}
