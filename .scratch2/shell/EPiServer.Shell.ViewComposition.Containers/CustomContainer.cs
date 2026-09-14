namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       Represents a container that can be easily customized.
///       </summary>
public class CustomContainer : ContainerBase
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.CustomContainer" /> class.
	///       </summary>
	/// <param name="widgetType">Client side widget identifier</param>
	public CustomContainer(string widgetType)
		: base(widgetType)
	{
	}
}
