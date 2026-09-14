using System.Collections;
using System.Collections.Generic;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       A dictionary of settings used for adding configuration aspects to children of containers.
///       </summary>
public interface ISettingsDictionary : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
{
	/// <summary>
	///       Gets the settings that are to be persisted to storage.
	///       </summary>
	/// <returns>An <see cref="T:System.Collections.IDictionary" /> with the settings that can be personalized.</returns>
	IDictionary<string, object> GetPersonalizableSettings();

	/// <summary>
	///       Adds the settings specified in <para>values</para> to the settings collection.
	///       </summary>
	/// <param name="values">The values.</param>
	/// <param name="personalizable">if the settings should be personalizable.</param>
	void AddSettings(IDictionary<string, object> values, bool personalizable);

	/// <summary>
	///       Adds the specified <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> object containing persistance information.
	///       </summary>
	/// <param name="setting">The setting object to add.</param>
	void Add(Setting setting);

	/// <summary>
	///       Merges a range of settings, overwriting current values if existing.
	///       </summary>
	/// <param name="settings">The settings.</param>
	void MergeRange(Setting[] settings);

	/// <summary>
	///       Creates a deep copy of the settings dictionary.
	///       </summary>
	/// <value>The copied dictionary.</value>
	ISettingsDictionary Copy();
}
