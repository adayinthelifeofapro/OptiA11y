namespace EPiServer.Shell.Navigation;

/// <summary>
///       Helps in ordering menu items by providing a standard set of menu positions.
///       </summary>
public static class SortIndex
{
	/// <summary>
	///       Approximate index for the first position in the menu [-1000].
	///       </summary>
	public const int First = -1000;

	/// <summary>
	///       Approximate index for a early position in the menu [-100].
	///       </summary>
	public const int Early = -100;

	/// <summary>
	///       Approximate index for a normal position in the menu [0].
	///       </summary>
	public const int Normal = 0;

	/// <summary>
	///       Approximate index for a late position in the menu [100].
	///       </summary>
	public const int Late = 100;

	/// <summary>
	///       Approximate index for the last position in the menu [1000].
	///       </summary>
	public const int Last = 1000;
}
