using System.Globalization;

namespace EPiServer.Web.Routing;

/// <summary>
///       This class supports the EPiServer infrastructure and is not intended to be used directly from your code.
///       </summary>
/// <exclude />
public class EditUrlArguments
{
	/// <summary>
	///       Gets or sets a value indicating whether to force the use of a edit host if present (but never primary site hosts)
	///       </summary>
	/// <remarks>
	///   <para>The primary use case for this property is linking to edit mode from templates.</para>
	///   <para>Note that the edit host will always be preferred if requested edit URL isn't for the current site.</para>
	/// </remarks>
	public bool ForceEditHost { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether to force the use of a site host (prefers edit hosts but fallbacks to primary host)
	///       </summary>
	/// <remarks>
	///   <para>The primary use case for this property is linking from external systems, such as links in e-mails.</para>
	///   <para>If language is specified hosts from the specified language will used, but fallbacks to hosts not locked to languages.</para>
	///   <para>Note that if no site can be detected an URL without host will be returned, use an overload that takes <see cref="T:EPiServer.Web.SiteDefinition" /> to be sure a host can be resolved. </para>
	/// </remarks>
	public bool ForceHost { get; set; }

	/// <summary>
	///       Gets or sets the specific language that should be requested.
	///       </summary>
	/// <remarks>This property will add language information to the URL but also try to select a language specific host if matched with ForceHost property.</remarks>
	public CultureInfo Language { get; set; }

	/// <summary>
	///       Gets or sets the name of the module that should be used for the edit URL. The default is 'CMS'.
	///       </summary>
	public string ModuleName { get; set; } = "CMS";

	/// <summary>
	///       Gets or sets a module relative path that should be used for the edit URL.
	///       </summary>
	public string ModuleRelativePath { get; set; }
}
