using System;

namespace EPiServer.Shell.Web;

/// <summary>
///       Abstract base class for attributes concerning required client resources
///       </summary>
public abstract class ClientResourceAttribute : Attribute
{
	/// <summary>
	///       Relative URL
	///       </summary>
	public string ResourceUrl { get; protected set; }

	/// <summary>
	///       The module were the resource can be found
	///       </summary>
	public string ModuleName { get; protected set; }

	/// <summary>
	///       Gets or sets the sort index, use this if you have dependencies between your resources.
	///       </summary>
	public int SortIndex { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.ClientResourceAttribute" /> class.
	///       </summary>
	/// <param name="resourceUrl">Relative URL.</param>
	protected ClientResourceAttribute(string resourceUrl)
		: this(resourceUrl, string.Empty)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.ClientResourceAttribute" /> class.
	///       </summary>
	/// <param name="resourceUrl">Relative URL.</param>
	/// <param name="moduleName">The module were the resource can be found.</param>
	protected ClientResourceAttribute(string resourceUrl, string moduleName)
	{
		ResourceUrl = resourceUrl;
		ModuleName = moduleName;
	}
}
