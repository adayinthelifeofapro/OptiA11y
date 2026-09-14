using System.Xml.Serialization;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       A serializable key-value class
///       </summary>
public class KeyValueElement
{
	/// <summary>
	///       The key
	///       </summary>
	[XmlAttribute("key")]
	public string Key { get; set; }

	/// <summary>
	///       The value
	///       </summary>
	[XmlAttribute("value")]
	public string Value { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Configuration.KeyValueElement" /> class.
	///       </summary>
	public KeyValueElement()
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Configuration.KeyValueElement" /> class.
	///       </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	public KeyValueElement(string key, string value)
	{
		Key = key;
		Value = value;
	}
}
