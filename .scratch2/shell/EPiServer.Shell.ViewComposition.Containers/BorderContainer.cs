namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       A layout container with the possibility of having up to five regions. Top, Bottom, Leading and Center
///       </summary>
public class BorderContainer : ContainerBase<BorderSettingsDictionary>
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderContainer" /> class.
	///       </summary>
	/// <param name="settings">Optional settings.</param>
	public BorderContainer(params Setting[] settings)
		: this("height: 100%; width: 100%;", settings)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderContainer" /> class.
	///       </summary>
	/// <param name="style">The css style.</param>
	/// <param name="settings">Optional settings.</param>
	public BorderContainer(string style, params Setting[] settings)
		: this(style, liveSplitters: true, gutters: false, settings)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderContainer" /> class.
	///       </summary>
	/// <param name="style">The style.</param>
	/// <param name="liveSplitters">if set to <c>true</c> containers will change layout live when resize.</param>
	/// <param name="gutters">if set to <c>true</c> containers will have borders even if they're not draggable.</param>
	/// <param name="settings">Optional settings.</param>
	public BorderContainer(string style, bool liveSplitters, bool gutters, params Setting[] settings)
		: base("dijit/layout/BorderContainer")
	{
		base.Settings["liveSplitters"] = liveSplitters;
		base.Settings["style"] = style;
		base.Settings["gutters"] = gutters;
		base.Settings.MergeRange(settings);
	}
}
