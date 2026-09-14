using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Search;

/// <summary>
///       Reoresents a registry where <see cref="T:System.IServiceProvider" /> instances can be registered/unregistered.
///       </summary>
public class SearchProviderRegistry : ISearchProviderSource
{
	private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

	private readonly ConcurrentDictionary<string, ISearchProvider> _searchProviders = new ConcurrentDictionary<string, ISearchProvider>(StringComparer.OrdinalIgnoreCase);

	/// <inherit-doc />
	public IEnumerable<ISearchProvider> List()
	{
		return _searchProviders.Values;
	}

	/// <inherit-doc />
	public IChangeToken GetChangeToken()
	{
		return new CancellationChangeToken(_cancellationTokenSource.Token);
	}

	/// <summary>
	///       Registers a <see cref="T:EPiServer.Shell.Search.ISearchProvider" />.
	///       </summary>
	public void Register(ISearchProvider searchProvider)
	{
		if (searchProvider.Key.Contains('.'))
		{
			throw new ArgumentException("Key cannot contain '.' character");
		}
		if (!_searchProviders.TryAdd(searchProvider.Key, searchProvider))
		{
			throw new InvalidOperationException("There already exist a search provider with key '" + searchProvider.Key + "'.");
		}
		SignalChange();
	}

	/// <summary>
	///       Unregisters a previously registered <see cref="T:EPiServer.Shell.Search.ISearchProvider" />.
	///       </summary>
	public void Unregister(ISearchProvider searchProvider)
	{
		_searchProviders.TryRemove(searchProvider.Key, out var _);
		SignalChange();
	}

	private void SignalChange()
	{
		CancellationTokenSource cancellationTokenSource = _cancellationTokenSource;
		_cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.Cancel();
		cancellationTokenSource.Dispose();
	}
}
