using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Implements sorting extesions for dynamic column names and sort order defined by the <see cref="T:EPiServer.Shell.Services.Rest.SortColumn" /> struct
///       and ranging extensions for using the <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" /> class.
///       </summary>
public static class CollectionExtensions
{
	/// <summary>
	///       Orders the <see cref="T:System.Linq.IQueryable`1" /> source by the directives in the sort parameter.
	///       </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <param name="source">The source containing the items to sort.</param>
	/// <param name="sortColumn">The sort column definition.</param>
	/// <returns>The <paramref name="source" /> with the sort expression appended</returns>
	/// <exception cref="T:System.ArgumentNullException">if <paramref name="source" /> is <c>null</c></exception>
	public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, SortColumn sortColumn)
	{
		return source.OrderBy(sortColumn, first: true);
	}

	/// <summary>
	///       Orders the <see cref="T:System.Linq.IQueryable`1" /> source by the directives in the sort parameter.
	///       </summary>
	/// <typeparam name="TSource">The type of the source.</typeparam>
	/// <param name="source">The source containing the items to sort.</param>
	/// <param name="sortColumn">The sort column definition.</param>
	/// <param name="first">if set to <c>true</c> the sort expression is treated as a primary sorting; otherwise secondary sorting is appended.</param>
	/// <returns>
	///       The <paramref name="source" /> with the sort expression appended
	///       </returns>
	/// <exception cref="T:System.ArgumentNullException">if <paramref name="source" /> is <c>null</c></exception>
	private static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, SortColumn sortColumn, bool first)
	{
		ArgumentNullException.ThrowIfNull(source, "source");
		ParameterExpression parameterExpression = Expression.Parameter(typeof(TSource));
		MemberExpression memberExpression = Expression.Property(parameterExpression, sortColumn.ColumnName);
		string methodName = ((!first) ? (sortColumn.SortDescending ? "ThenByDescending" : "ThenBy") : (sortColumn.SortDescending ? "OrderByDescending" : "OrderBy"));
		Expression expression = source.Expression;
		expression = Expression.Call(typeof(Queryable), methodName, new Type[2] { source.ElementType, memberExpression.Type }, expression, Expression.Quote(Expression.Lambda(memberExpression, parameterExpression)));
		return (IQueryable<TSource>)source.Provider.CreateQuery(expression);
	}

	/// <summary>
	///       Orders the <see cref="T:System.Linq.IQueryable`1" /> using a collection of <see cref="T:EPiServer.Shell.Services.Rest.SortColumn" />s.
	///       </summary>
	/// <param name="source">The source.</param>
	/// <param name="sortColumns">The sort columns.</param>
	/// <returns>The <paramref name="source" /> with the sort expression(s) appended</returns>
	/// <exception cref="T:System.ArgumentNullException">if <paramref name="source" /> or <paramref name="sortColumns" /> is <c>null</c></exception>
	public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, IEnumerable<SortColumn> sortColumns)
	{
		ArgumentNullException.ThrowIfNull(sortColumns, "sortColumns");
		ArgumentNullException.ThrowIfNull(source, "source");
		bool first = true;
		foreach (SortColumn sortColumn in sortColumns)
		{
			source = source.OrderBy(sortColumn, first);
			first = false;
		}
		return source;
	}

	/// <summary>
	///       Applies the specified range (start, end) and returns the items
	///       in the range together with the actual range (start, end, total).
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <param name="requestedRange">The requested range with start and/or end set.
	///       Null or not defined (neither start nor end set) to return the full range.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).</returns>
	public static RangedItems<T> ApplyRange<T>(this IEnumerable<T> items, ItemRange requestedRange)
	{
		if (requestedRange == null)
		{
			requestedRange = new ItemRange();
		}
		return requestedRange.ApplyTo(items);
	}

	/// <summary>
	///       Applies the specified range (start, end) and returns the items
	///       in the range together with the actual range (start, end, total).
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <param name="requestedRange">The requested range with start and/or end set.
	///       Null or not defined (neither start nor end set) to return the full range.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).</returns>
	public static RangedItems<T> ApplyRange<T>(this IQueryable<T> items, ItemRange requestedRange)
	{
		if (requestedRange == null)
		{
			requestedRange = new ItemRange();
		}
		return requestedRange.ApplyTo(items);
	}

	/// <summary>
	///       Applies the specified range (start, end) and returns the items
	///       in the range together with the actual range (start, end, total), using a
	///       supplied total count which means that <paramref name="items" /> don't have to
	///       be enumerated to get the count.
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <param name="requestedRange">The requested range with start and/or end set.
	///       Null or not defined (neither start nor end set) to return the full range.</param>
	/// <param name="totalItemCount">The total item count, or null if it should be
	///       reported as unknown. To get the actual total count by enumerating <paramref name="items" />,
	///       use the overload without the <paramref name="totalItemCount" /> parameter.</param>
	/// <returns>
	///       A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).
	///       </returns>
	public static RangedItems<T> ApplyRange<T>(this IEnumerable<T> items, ItemRange requestedRange, int? totalItemCount)
	{
		if (requestedRange == null)
		{
			requestedRange = new ItemRange();
		}
		return requestedRange.ApplyTo(items, totalItemCount);
	}

	/// <summary>
	///       Applies the specified range (start, end) and returns the items
	///       in the range together with the actual range (start, end, total), using a
	///       supplied total count which means that <paramref name="items" /> don't have to
	///       be enumerated to get the count.
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <param name="requestedRange">The requested range with start and/or end set.
	///       Null or not defined (neither start nor end set) to return the full range.</param>
	/// <param name="totalItemCount">The total item count, or null if it should be
	///       reported as unknown. To get the actual total count by enumerating <paramref name="items" />,
	///       use the overload without the <paramref name="totalItemCount" /> parameter.</param>
	/// <returns>
	///       A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).
	///       </returns>
	public static RangedItems<T> ApplyRange<T>(this IQueryable<T> items, ItemRange requestedRange, int? totalItemCount)
	{
		if (requestedRange == null)
		{
			requestedRange = new ItemRange();
		}
		return requestedRange.ApplyTo(items, totalItemCount);
	}
}
