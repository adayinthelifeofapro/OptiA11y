namespace EPiServer.Shell.ViewComposition.Internal;

/// <summary>
///       Used to define if a component supports a custom stylesheet which can be used to adjust the default page layout
///       </summary>
public interface ICustomizedView
{
	/// <summary>
	///       relative or absolute URL to the custom stylesheet
	///       </summary>
	string CustomStylesheetUrl { get; }
}
