using System.Collections.Generic;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       A marker class implementation of <see cref="T:System.Collections.Generic.Dictionary`2" /> for handling serialization of JSON formatted
///       dictionaries on requests to the REST stores.
///       </summary>
public class JsonDictionary : Dictionary<string, object>
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Services.Rest.JsonDictionary" /> class.
	///       </summary>
	public JsonDictionary()
	{
	}
}
