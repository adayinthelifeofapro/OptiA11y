using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Conveys information about the current page index, and helps iterating
///       pages to display in a UI.
///       </summary>
public class Pager
{
	/// <summary>
	///       The current page index.
	///       </summary>
	public int CurrentPageIndex { get; set; }

	/// <summary>
	///       The number of items per page.
	///       </summary>
	public int PageSize { get; set; }

	/// <summary>
	///       The total number of items in the set.
	///       </summary>
	public int TotalItemsCount { get; set; }

	/// <summary>
	///       Calculated value, number of items skipped before the first is relevant.
	///       </summary>
	public int SkipCount => CurrentPageIndex * PageSize;

	/// <summary>
	///       Calculated value, the number of pages for this set of items based on
	///       number of items and page size.
	///       </summary>
	public int TotalPageCount => (TotalItemsCount + PageSize - 1) / PageSize;

	/// <summary>
	///       Enumerates relevant pages depending on page size and current page index.
	///       </summary>
	/// <returns>En enumeration of pages that should be displayed.</returns>
	public IEnumerable<PagerPosition> VisiblePages
	{
		get
		{
			int num = 10;
			if (TotalPageCount <= num)
			{
				return Enumerable.Range(0, TotalPageCount).Select(PositionFromIndex);
			}
			return EnumerateVisiblePages(num);
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Web.Mvc.Html.Pager" /> class. Default value for PageSize is 10.
	///       </summary>
	public Pager()
	{
		PageSize = 10;
	}

	private IEnumerable<PagerPosition> EnumerateVisiblePages(int treshold)
	{
		int secondPageIndex = 1;
		int lastPageIndex = TotalPageCount - 1;
		int penultimatePageIndex = lastPageIndex - 1;
		int num = 3;
		int startIndex = CurrentPageIndex - treshold / 2 + 2;
		if (startIndex < secondPageIndex)
		{
			startIndex = secondPageIndex;
		}
		int stopIndex = startIndex + treshold - num;
		if (stopIndex >= TotalPageCount - num + 1)
		{
			startIndex = TotalPageCount - treshold + 1;
			stopIndex = TotalPageCount - num + 1;
		}
		yield return PositionFromIndex(0);
		PagerPosition pagerPosition = PositionFromIndex(startIndex);
		if (startIndex != secondPageIndex)
		{
			pagerPosition.Name = "...";
		}
		yield return pagerPosition;
		for (int i = startIndex + 1; i < stopIndex; i++)
		{
			yield return PositionFromIndex(i);
		}
		PagerPosition pagerPosition2 = PositionFromIndex(stopIndex);
		if (stopIndex != penultimatePageIndex)
		{
			pagerPosition2.Name = "...";
		}
		yield return pagerPosition2;
		yield return PositionFromIndex(lastPageIndex);
	}

	private PagerPosition PositionFromIndex(int index)
	{
		return new PagerPosition(index, (index + 1).ToString(CultureInfo.InvariantCulture), index == CurrentPageIndex);
	}
}
