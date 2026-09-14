using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Services.Rest;

namespace EPiServer.Shell.Rest;

/// <summary>
///       Container for items collection and a <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" /> instance
///       describing how the collection was selected from the corresponding total collection.
///       </summary>
public class QueryRange<T>
{
	/// <summary>
	///       The collection items.
	///       </summary>
	/// <remarks>Can be null!</remarks>
	public ICollection<T> Items { get; private set; }

	/// <summary>
	///       The resulting range
	///       </summary>
	public ItemRange Range { get; private set; }

	/// <summary>
	///       Initializes a new instance and sets the items collection and the resulting range.
	///       </summary>
	/// <param name="items">The items collection</param>
	/// <param name="range">The resulting range</param>
	public QueryRange(IEnumerable<T> items, ItemRange range)
	{
		if (items != null)
		{
			Items = items.ToList();
		}
		Range = range;
	}
}
