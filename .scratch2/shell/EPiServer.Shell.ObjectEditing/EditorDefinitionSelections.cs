using System.Collections.Generic;
using System.Text.Json.Serialization;
using EPiServer.Shell.ObjectEditing.Internal;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///        NOTE: This is a pre-release API that is UNSTABLE and might not satisfy the compatibility requirements as denoted by its associated normal version.
///
///        Used to define selections on a editor definition
///        </summary>
[JsonConverter(typeof(EditorDefinitionSelectionsConverter))]
public class EditorDefinitionSelections : List<KeyValuePair<string, object>>
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinitionSelections" /> class.
	///       </summary>
	public EditorDefinitionSelections()
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinitionSelections" /> class.
	///       </summary>
	/// <param name="collection">Initial values</param>
	public EditorDefinitionSelections(IEnumerable<KeyValuePair<string, object>> collection)
		: base(collection)
	{
	}

	/// <summary>
	///       Adds the specified key and value
	///       </summary>
	/// <param name="key">The key of the element to add</param>
	/// <param name="value">The value of the element to add</param>
	public void Add(string key, object value)
	{
		Add(new KeyValuePair<string, object>(key, value));
	}
}
