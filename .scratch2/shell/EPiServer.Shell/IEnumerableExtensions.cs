using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell;

internal static class IEnumerableExtensions
{
	/// <summary>
	///       Returns the first element of the specified type in the list
	///       </summary>
	/// <typeparam name="TType">The type to return.</typeparam>
	/// <param name="list">The list to operate on.</param>
	/// <returns>
	/// </returns>
	public static TType FirstOfType<TType>(this IEnumerable list)
	{
		return list.OfType<TType>().FirstOrDefault();
	}

	/// <summary>
	///       Sorts the source items in topological way (i.e. graph -&gt; from roots to leaves) Kahn's algorithm
	///       </summary>
	public static IList<T> TopologicalSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> getDependencies, IEqualityComparer<T> comparer = null) where T : class
	{
		List<T> list = new List<T>();
		Dictionary<T, bool> visited = new Dictionary<T, bool>(comparer);
		foreach (T item in source)
		{
			Visit(item, getDependencies, list, visited);
		}
		return list;
	}

	private static void Visit<T>(T item, Func<T, IEnumerable<T>> getDependencies, List<T> sorted, Dictionary<T, bool> visited)
	{
		if (visited.TryGetValue(item, out var _))
		{
			return;
		}
		visited[item] = true;
		IEnumerable<T> enumerable = getDependencies(item);
		if (enumerable != null)
		{
			foreach (T item2 in enumerable)
			{
				Visit(item2, getDependencies, sorted, visited);
			}
		}
		visited[item] = false;
		sorted.Add(item);
	}
}
