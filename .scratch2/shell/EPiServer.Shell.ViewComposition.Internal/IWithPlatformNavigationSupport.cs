namespace EPiServer.Shell.ViewComposition.Internal;

/// <summary>
///       Used to define if a component supports rendering Platform Navigation menu
///       </summary>
public interface IWithPlatformNavigationSupport
{
	bool SupportsPlatformNavigation { get; }
}
