using System;
using EPiServer.Applications;
using EPiServer.Core;

namespace EPiServer.Web.Routing;

/// <summary>
///       This class supports the EPiServer infrastructure and is not intended to be used directly from your code.
///       </summary>
/// <exclude />
public abstract class EditUrlResolver
{
	/// <summary>
	///       Gets the URL to the edit view of the current site.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	public virtual Url GetEditViewUrl()
	{
		return ResolveUrl((Application)null, (ContentReference)null, (EditUrlArguments)null);
	}

	/// <summary>
	///       Gets the URL to the edit view of the provided site.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	/// <exception cref="T:System.ArgumentNullException">site</exception>
	[Obsolete("Use different overload.", true)]
	public virtual Url GetEditViewUrl(SiteDefinition site)
	{
		ArgumentNullException.ThrowIfNull(site, "site");
		return ResolveUrl(site, null, null);
	}

	/// <summary>
	///       Gets the URL to the edit view of the provided site.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	/// <exception cref="T:System.ArgumentNullException">site</exception>
	public virtual Url GetEditViewUrl(Application application)
	{
		ArgumentNullException.ThrowIfNull(application, "application");
		return ResolveUrl(application, null, null);
	}

	/// <summary>
	///       Gets the URL to the edit view of the current site using the specified arguments.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	/// <exception cref="T:System.ArgumentNullException" />
	public virtual Url GetEditViewUrl(EditUrlArguments editUrlArguments)
	{
		ArgumentNullException.ThrowIfNull(editUrlArguments, "editUrlArguments");
		return ResolveUrl((Application)null, (ContentReference)null, editUrlArguments);
	}

	/// <summary>
	///       Gets the URL to the edit view of the provided site using the specified arguments.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	/// <exception cref="T:System.ArgumentNullException" />
	[Obsolete("Use different overload.", true)]
	public virtual Url GetEditViewUrl(SiteDefinition site, EditUrlArguments editUrlArguments)
	{
		ArgumentNullException.ThrowIfNull(site, "site");
		ArgumentNullException.ThrowIfNull(editUrlArguments, "editUrlArguments");
		return ResolveUrl(site, null, editUrlArguments);
	}

	/// <summary>
	///       Gets the URL to the edit view of the provided site using the specified arguments.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	/// <exception cref="T:System.ArgumentNullException" />
	public virtual Url GetEditViewUrl(Application application, EditUrlArguments editUrlArguments)
	{
		ArgumentNullException.ThrowIfNull(application, "application");
		ArgumentNullException.ThrowIfNull(editUrlArguments, "editUrlArguments");
		return ResolveUrl(application, null, editUrlArguments);
	}

	/// <summary>
	///       Gets the URL to the edit view for the specified content.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	/// <exception cref="T:System.ArgumentNullException" />
	public virtual Url GetEditViewUrl(ContentReference contentLink)
	{
		if (ContentReference.IsNullOrEmpty(contentLink))
		{
			throw new ArgumentNullException("contentLink");
		}
		return ResolveUrl((Application)null, contentLink, (EditUrlArguments)null);
	}

	/// <summary>
	///       Gets the URL to the edit view for the specified content and arguments.
	///       </summary>
	/// <returns>An absolute or relative URL</returns>
	/// <exception cref="T:System.ArgumentNullException" />
	public virtual Url GetEditViewUrl(ContentReference contentLink, EditUrlArguments editUrlArguments)
	{
		if (ContentReference.IsNullOrEmpty(contentLink))
		{
			throw new ArgumentNullException("contentLink");
		}
		ArgumentNullException.ThrowIfNull(editUrlArguments, "editUrlArguments");
		return ResolveUrl((Application)null, contentLink, editUrlArguments);
	}

	/// <summary>
	///       Should get the URL to the edit view for the specified site, content and arguments.
	///       </summary>
	/// <param name="site">The site. A null value implies the current site.</param>
	/// <param name="contentLink">The content link. A null value implies that the base edit view URL should be returned.</param>
	/// <param name="editUrlArguments">Arguments to use for resolving the URL</param>
	/// <returns>An absolute or relative URL</returns>
	[Obsolete("Use different overload.", true)]
	protected virtual Url ResolveUrl(SiteDefinition site, ContentReference contentLink, EditUrlArguments editUrlArguments)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	///       Should get the URL to the edit view for the specified site, content and arguments.
	///       </summary>
	/// <param name="application">The application. A null value implies the current application.</param>
	/// <param name="contentLink">The content link. A null value implies that the base edit view URL should be returned.</param>
	/// <param name="editUrlArguments">Arguments to use for resolving the URL</param>
	/// <returns>An absolute or relative URL</returns>
	protected abstract Url ResolveUrl(Application application, ContentReference contentLink, EditUrlArguments editUrlArguments);
}
