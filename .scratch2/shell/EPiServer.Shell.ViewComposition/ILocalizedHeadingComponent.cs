namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a component that has a localization key for the heading.
///       </summary>
internal interface ILocalizedHeadingComponent
{
	/// <summary>
	///       Gets or sets the heading for the component.
	///       </summary>
	string Heading { get; set; }

	/// <summary>
	///       Gets or sets the localization key for the heading.
	///       </summary>
	string HeadingLocalizationKey { get; set; }
}
