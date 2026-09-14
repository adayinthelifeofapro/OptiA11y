namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines the view
///       </summary>
public interface ICompositeView
{
	/// <summary>
	///       Gets the name of the view. Used or finding views.
	///       </summary>
	/// <value>The name.</value>
	/// <remarks>The name of the name should be unique. For instance "/episerver/dashboard" or "/partnername/customview".</remarks>
	string Name { get; }

	/// <summary>
	///       Gets a localized title for this view
	///       </summary>
	string Title { get; }

	/// <summary>
	///       Defines a default context for the view, for instance the start page for the CMS home view.
	///       </summary>
	/// <remarks>Set to null if the view should not have a default context.</remarks>
	string DefaultContext { get; }

	/// <summary>
	///       Gets the root <see cref="T:EPiServer.Shell.ViewComposition.IContainer" /> for the view.
	///       </summary>
	/// <value>The container.</value>
	IContainer RootContainer { get; }

	/// <summary>
	///       Creates a new instance of the view.
	///       </summary>
	/// <returns>A new instance of the view.</returns>
	ICompositeView CreateView();
}
