using System;
using System.Collections.Generic;
using System.Web;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Defines a sort directive, column sort order, as received from a dojo.store.JsonRest instance
///       </summary>
public class SortColumn
{
	private const char _asc = '+';

	private const char _desc = '-';

	/// <summary>
	///       Name of the column to perform sorting on.
	///       </summary>
	public string ColumnName { get; set; }

	/// <summary>
	///       A flag indicating whether to sort ascending or descending.
	///       </summary>
	/// <value>
	///   <c>true</c> if sorting should be performed in descending order; otherwise, <c>false</c>.</value>
	public bool SortDescending { get; set; }

	/// <summary>
	///       Implements the operator == using the <see cref="M:EPiServer.Shell.Services.Rest.SortColumn.Equals(System.Object)" /> method.
	///       </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>
	///   <c>true</c> if the instances are equal. See <see cref="M:EPiServer.Shell.Services.Rest.SortColumn.Equals(System.Object)" />.</returns>
	public static bool operator ==(SortColumn left, SortColumn right)
	{
		return left.Equals(right);
	}

	/// <summary>
	///       Implements the operator != using the <see cref="M:EPiServer.Shell.Services.Rest.SortColumn.Equals(System.Object)" /> method.
	///       </summary>
	/// <param name="left">The left instance.</param>
	/// <param name="right">The right instance.</param>
	/// <returns>
	///   <c>true</c> if the instances are not equal. See <see cref="M:EPiServer.Shell.Services.Rest.SortColumn.Equals(System.Object)" />.</returns>
	public static bool operator !=(SortColumn left, SortColumn right)
	{
		return !left.Equals(right);
	}

	/// <summary>
	///       Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
	///       </summary>
	/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
	/// <returns>
	///   <c>true</c> if the <see name="ColumnName" /> and <see cref="P:EPiServer.Shell.Services.Rest.SortColumn.SortDescending" /> properties of the instances are equal; otherwise, <c>false</c>.
	///       </returns>
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		SortColumn sortColumn = (SortColumn)obj;
		if (ColumnName == sortColumn.ColumnName)
		{
			return SortDescending == sortColumn.SortDescending;
		}
		return false;
	}

	/// <summary>
	///       Calculates Returns a hash code by adding the hash code of the <see name="ColumnName" /> and the <see cref="P:EPiServer.Shell.Services.Rest.SortColumn.SortDescending" />.
	///       </summary>
	/// <returns>
	///       A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
	///       </returns>
	public override int GetHashCode()
	{
		return (ColumnName ?? string.Empty).GetHashCode() + SortDescending.GetHashCode();
	}

	/// <summary>
	///       Parses a string with comma-separated sort column and order information.
	///       </summary>
	/// <param name="sortQuery">The string containing sort information.</param>
	/// <example>Parse the string representation of sort information to a collection of <see cref="T:EPiServer.Shell.Services.Rest.SortColumn" /><code>
	///           var sortColumns = SortColumn.ParseSortList("+name,-title");
	///       </code></example>
	/// <returns>
	/// </returns>
	private static List<SortColumn> ParseSortList(string sortQuery)
	{
		if (string.IsNullOrEmpty(sortQuery))
		{
			return new List<SortColumn>();
		}
		string[] array = sortQuery.Split(',', StringSplitOptions.RemoveEmptyEntries);
		List<SortColumn> list = new List<SortColumn>(array.Length);
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Trim();
			char c = text[0];
			if ((c == '+' || c == '-') ? true : false)
			{
				text = text.Substring(1);
			}
			else
			{
				c = '+';
			}
			if (text.Length > 0)
			{
				list.Add(new SortColumn
				{
					SortDescending = (c == '-'),
					ColumnName = text
				});
			}
		}
		return list;
	}

	/// <summary>
	///       Tries to parse a string as a list of sort directives.
	///       </summary>
	/// <param name="sortRules">The query param containing the sort information.</param>
	/// <returns>
	///   <c>true</c> if the sort information in <paramref name="sortRules" /> was successfully parsed; otherwise <c>false</c></returns>
	public static IEnumerable<SortColumn> Parse(string sortRules)
	{
		sortRules = HttpUtility.UrlDecode(sortRules);
		int num = sortRules.IndexOf("sort(", StringComparison.OrdinalIgnoreCase);
		if (num < 0)
		{
			return null;
		}
		num += 5;
		int num2 = sortRules.IndexOf(")", num, StringComparison.OrdinalIgnoreCase);
		if (num2 < 0)
		{
			return null;
		}
		string text = sortRules;
		int num3 = num;
		sortRules = text.Substring(num3, num2 - num3);
		return ParseSortList(sortRules);
	}
}
