using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       A helper class for parsing the Range http header on incoming requests and writing the Items-Content-Range http header on outgoing responses.
///       </summary>
[ModelBinder(BinderType = typeof(ItemRangeModelBinder))]
public class ItemRange
{
	/// <summary>
	///       Gets or sets the start index of the requested or returned range of items.
	///       </summary>
	public int? Start { get; set; }

	/// <summary>
	///       Gets or sets the index of the last item requested or returned.
	///       </summary>
	public int? End { get; set; }

	/// <summary>
	///       Gets the calculated length (number of items) from Start to End.
	///       </summary>
	public long? Length
	{
		get
		{
			if (Start.HasValue && End.HasValue)
			{
				return Math.Max(0L, (long)End.Value - (long)Start.Value + 1);
			}
			return null;
		}
	}

	/// <summary>
	///       Gets or sets the total number of items. Only used in the response. Null if unknown.
	///       </summary>
	public int? Total { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" /> class.
	///       </summary>
	public ItemRange()
	{
	}

	/// <summary>
	///       Creates an <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" /> object from the headers in the specified http request.
	///       </summary>
	/// <param name="httpRequest">The HTTP request.</param>
	/// <returns>
	///       An <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" /> with information about the requested range; or null if the no range information was found in the headers.
	///       </returns>
	public static ItemRange ReadHeaderFrom(HttpRequest httpRequest)
	{
		ArgumentNullException.ThrowIfNull(httpRequest, "httpRequest");
		string text = httpRequest.Headers["ItemsRange"];
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		Match match = new Regex("items=(?<start>\\d+)-(?<end>\\d*)", RegexOptions.IgnoreCase).Match(text);
		if (match == null || !match.Success)
		{
			throw new FormatException("Invalid range header. Should be in the format items=0- or items=0-18.");
		}
		Group obj = match.Groups["start"];
		Group obj2 = match.Groups["end"];
		return new ItemRange
		{
			Start = int.Parse(obj.Value, CultureInfo.InvariantCulture),
			End = (string.IsNullOrEmpty(obj2.Value) ? ((int?)null) : new int?(int.Parse(obj2.Value, CultureInfo.InvariantCulture)))
		};
	}

	/// <summary>
	///       Adds the range information to the specified http response.
	///       </summary>
	/// <param name="httpResponse">The HTTP response.</param>
	public void AddHeaderTo(HttpResponse httpResponse)
	{
		ArgumentNullException.ThrowIfNull(httpResponse, "httpResponse");
		object arg = ((object)Total) ?? "*";
		httpResponse.Headers["Items-Content-Range"] = string.Format(CultureInfo.InvariantCulture, "items {0}-{1}/{2}", Start.GetValueOrDefault(), End.GetValueOrDefault(), arg);
	}

	/// <summary>
	///       Applies the range (start, end) and returns the items in the range together
	///       with the actual range (start, end, total).
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).</returns>
	public virtual RangedItems<T> ApplyTo<T>(IEnumerable<T> items)
	{
		if (items is IQueryable<T> items2)
		{
			return ApplyTo(items2);
		}
		List<T> list = items?.ToList() ?? new List<T>();
		return ApplyTo(list, list.Count);
	}

	/// <summary>
	///       Applies the range (start, end) and returns the items in the range together
	///       with the actual range (start, end, total). Utilizes the query engine of the
	///       IQueryable to avoid fetching all items unless needed.
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <returns>A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).</returns>
	public virtual RangedItems<T> ApplyTo<T>(IQueryable<T> items)
	{
		return ApplyTo(items, items?.Count() ?? 0);
	}

	/// <summary>
	///       Applies the range (start, end) and returns the items in the range together
	///       with the actual range (start, end, total), using a supplied total count
	///       which means that <paramref name="items" /> don't have to be enumerated to
	///       get the count.
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <param name="totalItemCount">The total item count, or null if it should be
	///       reported as unknown. To get the actual total count by enumerating <paramref name="items" />,
	///       use the overload without the <paramref name="totalItemCount" /> parameter.</param>
	/// <returns>
	///       A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).
	///       </returns>
	public virtual RangedItems<T> ApplyTo<T>(IEnumerable<T> items, int? totalItemCount)
	{
		if (items is IQueryable<T> items2)
		{
			return ApplyTo(items2, totalItemCount);
		}
		if (items == null)
		{
			items = Enumerable.Empty<T>();
		}
		if (!Start.HasValue && !totalItemCount.HasValue)
		{
			items = items.ToList();
		}
		return ApplyTo(items.AsQueryable(), totalItemCount);
	}

	/// <summary>
	///       Applies the range (start, end) and returns the items in the range together
	///       with the actual range (start, end, total), using a supplied total count
	///       which means that <paramref name="items" /> don't have to be enumerated to
	///       get the count.
	///       </summary>
	/// <typeparam name="T">The type of items to apply range to.</typeparam>
	/// <param name="items">The full item collection to get the range from.</param>
	/// <param name="totalItemCount">The total item count, or null if it should be
	///       reported as unknown. To get the actual total count by enumerating <paramref name="items" />,
	///       use the overload without the <paramref name="totalItemCount" /> parameter.</param>
	/// <returns>
	///       A <see cref="T:EPiServer.Shell.Services.Rest.RangedItems`1" /> instance containing the
	///       resulting range of items as well as a new <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" />
	///       instance describing the actual range (including total).
	///       </returns>
	public virtual RangedItems<T> ApplyTo<T>(IQueryable<T> items, int? totalItemCount)
	{
		ItemRange itemRange = new ItemRange
		{
			Total = totalItemCount
		};
		IQueryable<T> source = items ?? Enumerable.Empty<T>().AsQueryable();
		int valueOrDefault = Start.GetValueOrDefault();
		itemRange.Start = valueOrDefault;
		source = source.Skip(valueOrDefault);
		if (End.HasValue)
		{
			int value = End.Value;
			if (value >= valueOrDefault)
			{
				int count = value - valueOrDefault + 1;
				source = source.Take(count);
			}
		}
		int num = source.Count();
		itemRange.End = itemRange.Start + num - 1;
		return new RangedItems<T>(source, itemRange);
	}
}
