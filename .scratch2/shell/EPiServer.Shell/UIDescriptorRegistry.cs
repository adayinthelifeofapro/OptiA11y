using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell;

/// <summary>
///       Registry that holds all ui descriptors for entity types.
///       </summary>
public class UIDescriptorRegistry
{
	private enum InitializationState
	{
		NotInitialized,
		Initializing,
		Initialized
	}

	private class TypeComparer : Comparer<UIDescriptor>
	{
		public static readonly TypeComparer Comparer = new TypeComparer();

		/// <summary>
		///       Compares two UIDescriptors based on their connected type. X is greater than Y if X's instances are assignable from Y's instances
		///       </summary>
		/// <param name="x">The first UIDescriptor instance.</param>
		/// <param name="y">The second UIDescriptor instance.</param>
		/// <returns>
		/// </returns>
		public override int Compare(UIDescriptor x, UIDescriptor y)
		{
			if (x == y)
			{
				return 0;
			}
			Type forType = x.ForType;
			Type forType2 = y.ForType;
			if (forType.IsInterface && !forType2.IsInterface)
			{
				return 1;
			}
			if (forType.IsAssignableFrom(forType2))
			{
				return 1;
			}
			return -1;
		}
	}

	private readonly ILogger<UIDescriptorRegistry> _log;

	private readonly Lock _lock = new Lock();

	private InitializationState _state;

	private readonly List<UIDescriptor> _uiDescriptors;

	private List<UIDescriptor> _resolvedDescriptors;

	private readonly IEnumerable<UIDescriptorProvider> _descriptorProviders;

	private CompositeChangeToken _changeToken;

	private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

	/// <summary>
	///       A collection of the <see cref="T:EPiServer.Shell.UIDescriptor" />s registered for available content types
	///       </summary>
	public virtual IEnumerable<UIDescriptor> UIDescriptors
	{
		get
		{
			if (_state == InitializationState.NotInitialized)
			{
				using (_lock.EnterScope())
				{
					if (_state == InitializationState.NotInitialized)
					{
						_state = InitializationState.Initializing;
						_resolvedDescriptors = InitializeDescriptors();
						_state = InitializationState.Initialized;
					}
				}
			}
			return _resolvedDescriptors;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UIDescriptorRegistry" /> class.
	///       </summary>
	/// <param name="uiDescriptors">The UI descriptors.</param>
	/// <param name="descriptorProviders">A collection of providers queried for additiona descriptors</param>
	/// <param name="logger">The logger.</param>
	public UIDescriptorRegistry(IEnumerable<UIDescriptor> uiDescriptors, IEnumerable<UIDescriptorProvider> descriptorProviders, ILogger<UIDescriptorRegistry> logger)
	{
		_uiDescriptors = ((uiDescriptors == null) ? new List<UIDescriptor>() : uiDescriptors.ToList());
		_descriptorProviders = descriptorProviders ?? Enumerable.Empty<UIDescriptorProvider>();
		_log = logger;
		_changeToken = new CompositeChangeToken(_descriptorProviders.Select((UIDescriptorProvider p) => p.GetChangeToken()).ToArray());
		_changeToken.RegisterChangeCallback(ProviderChanged, null);
	}

	/// <summary>
	///       Defines a change token that can be used to signal changes.
	///       </summary>
	public virtual IChangeToken GetChangeToken()
	{
		using (_lock.EnterScope())
		{
			return new CancellationChangeToken(_cancellationTokenSource.Token);
		}
	}

	/// <summary>
	///       Gets the type identifiers.
	///       </summary>
	/// <param name="type">The type to find .</param>
	/// <returns>A sorted list with the most specific matching type first.</returns>
	public virtual IEnumerable<string> GetTypeIdentifiers(Type type)
	{
		return from u in GetDescriptorsForType(type)
			select u.TypeIdentifier;
	}

	/// <summary>
	///       Gets a list of ui descriptors registered for a type and its base types.
	///       </summary>
	/// <param name="type">
	/// </param>
	/// <returns>A sorted list with the most specific matching type first.</returns>
	public virtual IEnumerable<UIDescriptor> GetDescriptorsForType(Type type)
	{
		return GetDescriptorsForType(type, UIDescriptors);
	}

	/// <summary>
	///       Determines whether any ui descriptor matching the type has any matching type identifiers in the provided collection.
	///       </summary>
	/// <param name="type">The type to get ui descriptors for.</param>
	/// <param name="typeIdentifiers">A collection of type identifier for matching.</param>
	public virtual bool HasMatchedTypeIdentifier(Type type, IEnumerable<string> typeIdentifiers)
	{
		return GetDescriptorsForType(type).Any((UIDescriptor t) => typeIdentifiers.Contains(t.TypeIdentifier));
	}

	private static void SetBaseTypes(UIDescriptor descriptor, IList<UIDescriptor> descriptors)
	{
		List<UIDescriptor> list = new List<UIDescriptor>();
		Type forType = descriptor.ForType;
		Type[] interfaces = forType.GetInterfaces();
		Type baseType = forType.BaseType;
		UIDescriptor uIDescriptor = GetDescriptorsForType(baseType, descriptors).FirstOrDefault();
		if (uIDescriptor != null)
		{
			list.Add(uIDescriptor);
		}
		if (interfaces.Length != 0)
		{
			IEnumerable<Type> enumerable = interfaces.Where((Type x) => !interfaces.Any((Type y) => y.GetInterfaces().Contains(x, null)));
			if (baseType != null)
			{
				enumerable = enumerable.Except(baseType.GetInterfaces());
			}
			foreach (Type item in enumerable)
			{
				uIDescriptor = GetDescriptorsForType(item, descriptors).FirstOrDefault();
				if (uIDescriptor != null)
				{
					list.Add(uIDescriptor);
				}
			}
		}
		descriptor.BaseTypes = list.Select((UIDescriptor d) => d.TypeIdentifier).Distinct().ToList();
		descriptor.BaseTypeIdentifier = descriptor.BaseTypes.FirstOrDefault();
	}

	/// <summary>
	///       Gets an ordered list of any descriptors for a particular type or any of its base types.
	///       The list is ordered by inheritance with the most specific first.
	///       </summary>
	/// <param name="type">The type to look for</param>
	/// <param name="descriptors">A collection of descriptors to search</param>
	/// <returns>An ordered list of matching descriptors</returns>
	private static IEnumerable<UIDescriptor> GetDescriptorsForType(Type type, IEnumerable<UIDescriptor> descriptors)
	{
		return descriptors.Where((UIDescriptor u) => u.ForType.IsAssignableFrom(type)).OrderBy((UIDescriptor u) => u, TypeComparer.Comparer);
	}

	private List<UIDescriptor> InitializeDescriptors()
	{
		List<IGrouping<string, UIDescriptor>> list = (from x in _uiDescriptors
			group x by x.TypeIdentifier).ToList();
		foreach (IGrouping<string, UIDescriptor> item in list)
		{
			if (item.Count() > 1)
			{
				IEnumerable<string> values = from x in item.Skip(1)
					select x.GetType().FullName;
				_log?.DuplicateUIDescriptorsDiscarded(string.Join(", ", values));
			}
		}
		Dictionary<string, UIDescriptor> dictionary = list.Select((IGrouping<string, UIDescriptor> x) => x.FirstOrDefault()).ToDictionary<UIDescriptor, string>((UIDescriptor descriptor) => descriptor.TypeIdentifier, StringComparer.OrdinalIgnoreCase);
		foreach (UIDescriptorProvider descriptorProvider in _descriptorProviders)
		{
			foreach (UIDescriptor descriptor in descriptorProvider.GetDescriptors())
			{
				string typeIdentifier = descriptor.TypeIdentifier;
				if (!dictionary.TryAdd(typeIdentifier, descriptor))
				{
					_log?.UIDescriptorDiscarded(typeIdentifier, descriptorProvider.GetType().FullName);
				}
			}
		}
		foreach (UIDescriptor value in dictionary.Values)
		{
			SetBaseTypes(value, dictionary.Values.ToList());
		}
		return dictionary.Values.ToList();
	}

	private void ProviderChanged(object obj)
	{
		CancellationTokenSource cancellationTokenSource;
		using (_lock.EnterScope())
		{
			_state = InitializationState.NotInitialized;
			_changeToken = new CompositeChangeToken(_descriptorProviders.Select((UIDescriptorProvider p) => p.GetChangeToken()).ToArray());
			_changeToken.RegisterChangeCallback(ProviderChanged, null);
			cancellationTokenSource = _cancellationTokenSource;
			_cancellationTokenSource = new CancellationTokenSource();
		}
		cancellationTokenSource.Cancel();
	}
}
