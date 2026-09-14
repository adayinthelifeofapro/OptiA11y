namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       A general purpose pane capable of loading external data from a url
///       </summary>
[Component(IsAvailableForUserSelection = false)]
public class ContentPane : ContainerBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.ContentPane" /> class.
	///       </summary>
	public ContentPane()
		: this((Setting[])null)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.ContentPane" /> class with the specified settings.
	///       </summary>
	/// <param name="settings">The settings.</param>
	public ContentPane(params Setting[] settings)
		: this(null, extractContent: false, parseOnLoad: false, preventCache: false, preload: false, refreshOnShow: false, null, null, settings)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.ContentPane" /> class.
	///       </summary>
	/// <param name="href">The href.</param>
	/// <param name="extractContent">if set to <c>true</c> extracts content from the body of what is loaded from href.</param>
	/// <param name="parseOnLoad">if set to <c>true</c> parses the content loaded for additional widgets.</param>
	/// <param name="preventCache">if set to <c>true</c> prevents clientside cahce of requests from the href.</param>
	/// <param name="preload">if set to <c>true</c> loads content from the href even if it is not visible.</param>
	/// <param name="refreshOnShow">if set to <c>true</c> refreshes content from the href when it becomes visible.</param>
	/// <param name="loadingMessage">A message to display while loading.</param>
	/// <param name="errorMessage">a message to display when loading fails.</param>
	/// <param name="settings">Optional settings.</param>
	public ContentPane(string href, bool extractContent, bool parseOnLoad, bool preventCache, bool preload, bool refreshOnShow, string loadingMessage, string errorMessage, params Setting[] settings)
		: base("dijit/layout/ContentPane")
	{
		if (settings != null)
		{
			if (href != null)
			{
				base.Settings["href"] = href;
			}
			if (extractContent)
			{
				base.Settings["extractContent"] = extractContent;
			}
			if (parseOnLoad)
			{
				base.Settings["parseOnLoad"] = parseOnLoad;
			}
			if (preventCache)
			{
				base.Settings["preventCache"] = preventCache;
			}
			if (preload)
			{
				base.Settings["preload"] = preload;
			}
			if (refreshOnShow)
			{
				base.Settings["refreshOnShow"] = refreshOnShow;
			}
			if (loadingMessage != null)
			{
				base.Settings["loadingMessage"] = loadingMessage;
			}
			if (errorMessage != null)
			{
				base.Settings["errorMessage"] = errorMessage;
			}
			base.Settings.MergeRange(settings);
		}
	}
}
