namespace EPiServer.Shell.Search;

/// <summary>
///       Contains constants used by search providers
///       </summary>
public static class SearchProviderConstants
{
	/// <summary>
	///       The lowest rated search provider. Having such sort order means that any other alternative implementation with
	///       the same searchArea will turn it off.
	///       </summary>
	public const int BuiltInSearchProviderSortOrder = int.MinValue;
}
