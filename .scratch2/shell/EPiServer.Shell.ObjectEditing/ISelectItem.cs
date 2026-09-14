namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Common interface for selections using on client object editing
///       </summary>
public interface ISelectItem
{
	/// <summary>
	///       Gets the value.
	///       </summary>
	object Value { get; }

	/// <summary>
	///       Gets the text.
	///       </summary>
	string Text { get; }
}
