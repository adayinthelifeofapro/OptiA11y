using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Contains a collection of items and a description of the range of items.
///       </summary>
/// <typeparam name="T">
/// </typeparam>
public class RangedItems<T>
{
	/// <summary>
	///       Gets the included items.
	///       </summary>
	public IEnumerable<T> Items { get; protected set; }

	/// <summary>
	///       Gets the range describing the included items.
	///       </summary>
	public ItemRange Range { get; protected set; }

	/// <summary>
	///       Creates a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> class.
	///       </summary>
	protected RangedItems()
	{
		Items = Enumerable.Empty<T>();
	}

	/// <summary>
	///       Creates a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> class.
	///       </summary>
	/// <param name="items">The items.</param>
	/// <param name="range">The range describing the included items.</param>
	public RangedItems(IEnumerable<T> items, ItemRange range)
	{
		if (items != null)
		{
			Items = items;
		}
		Range = range;
	}
}
