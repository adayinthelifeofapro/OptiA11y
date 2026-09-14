namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Creates a new <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> that will always be persisted.
///       </summary>
public class PersonalizableSetting : Setting
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizableSetting" /> class.
	///       </summary>
	/// <param name="key">The key for the setting.</param>
	public PersonalizableSetting(string key)
		: base(key, personalizable: true)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.PersonalizableSetting" /> class.
	///       </summary>
	/// <param name="key">The key for the setting.</param>
	/// <param name="value">The value for the setting.</param>
	public PersonalizableSetting(string key, object value)
		: base(key, value, personalizable: true)
	{
	}
}
