using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       A collection of settings used for adding configuration aspects for children of containers. 
///       Inherit to create implementations matching specific containers to declare the necessy settings. 
///       </summary>
[Serializable]
public class SettingsDictionary : ISettingsDictionary, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
{
	/// <summary>
	///       Enumerator implementation
	///       </summary>
	public sealed class Enumerator : IEnumerator<KeyValuePair<string, object>>, IEnumerator, IDisposable
	{
		private readonly Dictionary<string, Setting> _dictionary;

		private Dictionary<string, Setting>.Enumerator _innerEnumerator;

		/// <summary>
		///       Gets the current entry.
		///       </summary>
		public KeyValuePair<string, object> Current { get; private set; }

		/// <summary>
		///       Gets the current entry.
		///       </summary>
		/// <value>The current.</value>
		object IEnumerator.Current => Current;

		/// <summary>
		///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.SettingsDictionary.Enumerator" /> class.
		///       </summary>
		/// <param name="dictionary">The dictionary to enumerate.</param>
		public Enumerator(Dictionary<string, Setting> dictionary)
		{
			_dictionary = dictionary;
			_innerEnumerator = _dictionary.GetEnumerator();
			Current = CreateCurrent(_innerEnumerator.Current);
		}

		/// <summary>
		///       Creates the current.
		///       </summary>
		/// <param name="current">The current.</param>
		/// <returns>
		/// </returns>
		private static KeyValuePair<string, object> CreateCurrent(KeyValuePair<string, Setting> current)
		{
			return new KeyValuePair<string, object>(current.Key, current.Value?.Value);
		}

		/// <summary>
		///       Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		///       </summary>
		public void Dispose()
		{
		}

		/// <summary>
		///       Advances the enumerator to the next element of the collection.
		///       </summary>
		/// <returns>
		///       true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		///       </returns>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		public bool MoveNext()
		{
			bool result = _innerEnumerator.MoveNext();
			Current = CreateCurrent(_innerEnumerator.Current);
			return result;
		}

		/// <summary>
		///       Sets the enumerator to its initial position, which is before the first element in the collection.
		///       </summary>
		/// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
		public void Reset()
		{
			((IEnumerator)_innerEnumerator).Reset();
			Current = CreateCurrent(_innerEnumerator.Current);
		}
	}

	private readonly Dictionary<string, Setting> _dictionary = new Dictionary<string, Setting>();

	/// <summary>
	///       Specifies what Settings that should be merged in some fashion instead of being replaced.
	///       </summary>
	private static readonly IEnumerable<string> MergeableSettings = new List<string> { "style", "class" };

	/// <summary>
	///       Gets or sets the <see cref="T:System.Object" /> with the specified key.
	///       </summary>
	/// <value>
	/// </value>
	public object this[string key]
	{
		get
		{
			if (_dictionary.TryGetValue(key, out var value))
			{
				return value.Value;
			}
			return null;
		}
		set
		{
			if (value is Setting)
			{
				throw new ArgumentException("Setting is not allowed as value");
			}
			if (!_dictionary.TryGetValue(key, out var value2))
			{
				_dictionary.Add(key, new Setting(key, value));
				return;
			}
			Setting setting = value2;
			if (MergeableSettings.Contains(key) && setting.Value is string && value is string)
			{
				setting.Value = string.Format(CultureInfo.InvariantCulture, "{0} {1}", setting.Value, value);
			}
			else
			{
				setting.Value = value;
			}
		}
	}

	/// <summary>
	///       Gets the keys in the dictionary.
	///       </summary>
	public ICollection<string> Keys => _dictionary.Keys;

	/// <summary>
	///       Gets a collection containing the values in the dictionary.
	///       </summary>
	public ICollection<object> Values => _dictionary.Values.Select((Setting s) => s.Value).ToArray();

	/// <summary>
	///       Gets the number of elements contained in this dictionary.
	///       </summary>
	public int Count => _dictionary.Count;

	/// <summary>
	///       Gets a value indicating whether this instance is read only.
	///       </summary>
	/// <value>
	///   <c>true</c> if this instance is read only; otherwise, <c>false</c>.
	///       </value>
	public bool IsReadOnly => ((ICollection<KeyValuePair<string, Setting>>)_dictionary).IsReadOnly;

	/// <summary>
	///       Adds the specified <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> object containing persistance information. Will override any previous
	///       setting with the same key.
	///       </summary>
	/// <param name="setting">The setting object to add.</param>
	public void Add(Setting setting)
	{
		_dictionary.Remove(setting.Key);
		_dictionary.Add(setting.Key, setting);
	}

	/// <summary>
	///       Adds a range of <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> objects.
	///       </summary>
	/// <param name="settings">The settings.</param>
	public void MergeRange(Setting[] settings)
	{
		foreach (Setting setting in settings)
		{
			this[setting.Key] = setting.Value;
		}
	}

	/// <summary>
	///       Gets the settings that can be personalized.
	///       </summary>
	/// <returns>An <see cref="T:System.Collections.IDictionary" /> with the settings that can be personalized.</returns>
	public IDictionary<string, object> GetPersonalizableSettings()
	{
		return _dictionary.Values.Where((Setting s) => s.Personalizable).ToDictionary((Setting s) => s.Key, (Setting s) => s.Value);
	}

	/// <summary>
	///       Adds the settings specified in <para>values</para> to the settings collection.
	///       </summary>
	/// <param name="values">The values.</param>
	/// <param name="personalizable">if the settings should be personalizable.</param>
	public void AddSettings(IDictionary<string, object> values, bool personalizable)
	{
		if (values == null)
		{
			return;
		}
		foreach (KeyValuePair<string, object> value in values)
		{
			_dictionary[value.Key] = new Setting(value.Key, value.Value, personalizable);
		}
	}

	/// <summary>
	///       Creates a deep copy of the settings dictionary.
	///       </summary>
	/// <returns>A new settings dictionary with the same values as this object.</returns>
	/// <value>The copied dictionary.</value>
	public ISettingsDictionary Copy()
	{
		SettingsDictionary settingsDictionary = new SettingsDictionary();
		foreach (KeyValuePair<string, Setting> item in _dictionary)
		{
			settingsDictionary.Add(item.Value.Copy());
		}
		return settingsDictionary;
	}

	/// <summary>
	///       Adds the specified key and value to the dictionary.
	///       </summary>
	/// <remarks>
	///       The value will not be persisted. 
	///       </remarks>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	public void Add(string key, object value)
	{
		if (value is Setting)
		{
			throw new ArgumentException("Value must not inherit from PersistableSetting", "value");
		}
		_dictionary.Add(key, new Setting(key, value));
	}

	/// <summary>
	///       Determines whether the specified key exists in the dictionary.
	///       </summary>
	/// <param name="key">Name of the key.</param>
	/// <returns>
	///   <c>true</c> if the dictionary contains the key; otherwise, <c>false</c>.
	///       </returns>
	public bool ContainsKey(string key)
	{
		return _dictionary.ContainsKey(key);
	}

	/// <summary>
	///       Removes the value with the specified key from the dictionary.
	///       </summary>
	/// <param name="key">Name of the key to remove.</param>
	/// <returns>
	///   <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
	///       </returns>
	public bool Remove(string key)
	{
		return _dictionary.Remove(key);
	}

	/// <summary>
	///       Gets the value associated with the specified key.
	///       </summary>
	/// <param name="key">The key of the value to get.</param>
	/// <param name="value">Contains the value associated with the specified key if the key exists in the dictionary.</param>
	/// <returns>
	///   <c>true</c> if the dictionary contains an element with the specified key; otherwise, <c>false</c>.</returns>
	public bool TryGetValue(string key, out object value)
	{
		if (_dictionary.TryGetValue(key, out var value2))
		{
			value = value2.Value;
			return true;
		}
		value = null;
		return false;
	}

	/// <summary>
	///       Adds the specified key and value to the dictionary.
	///       </summary>
	/// <param name="item">The KeyValuePair to add</param>
	/// <remarks>
	///       The value will not be persisted.
	///       </remarks>
	public void Add(KeyValuePair<string, object> item)
	{
		Add(new Setting(item.Key, item.Value));
	}

	/// <summary>
	///       Remove all elements from the dictionary.
	///       </summary>
	public void Clear()
	{
		_dictionary.Clear();
	}

	/// <summary>
	///       Determines whether this dictionary contains the specified key value combination.
	///       </summary>
	/// <param name="item">The KeyValuePair to test.</param>
	/// <returns>
	///   <c>true</c> if the dictionary contains the specified KeyValuePair; otherwise, <c>false</c>.
	///       </returns>
	public bool Contains(KeyValuePair<string, object> item)
	{
		if (_dictionary.ContainsKey(item.Key))
		{
			return _dictionary[item.Key].Value == item.Value;
		}
		return false;
	}

	/// <summary>
	///       Copies the elements of the collection to an array, starting at a particular array index.
	///       </summary>
	/// <param name="array">The array which the elements are copied to.</param>
	/// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
	public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
	{
		ArgumentNullException.ThrowIfNull(array, "array");
		if (arrayIndex < 0 || array.Length - arrayIndex < _dictionary.Count)
		{
			throw new ArgumentException("Invalid index. Index is less than zero or out of bounds.");
		}
		using IEnumerator<KeyValuePair<string, object>> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, object> current = enumerator.Current;
			array[arrayIndex++] = current;
		}
	}

	/// <summary>
	///       Removes the specified item from the collection.
	///       </summary>
	/// <param name="item">The item to remove.</param>
	/// <returns>
	///   <c>true</c> if the item was found and successfully removed; otherwise <c>false</c>.</returns>
	public bool Remove(KeyValuePair<string, object> item)
	{
		return Remove(item.Key);
	}

	/// <summary>
	///       Gets the enumerator for iterating thhrough the collection.
	///       </summary>
	public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
	{
		return new Enumerator(_dictionary);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return new Enumerator(_dictionary);
	}
}
