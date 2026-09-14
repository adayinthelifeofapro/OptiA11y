using System;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Used for adding not commonly used settings for <see cref="T:EPiServer.Shell.ViewComposition.IComponent" />.
///       </summary>
public class Setting
{
	/// <summary>
	///       The value to insert into the settings collection
	///       </summary>
	public object Value { get; set; }

	/// <summary>
	///       The key to use in the settings collection
	///       </summary>
	public string Key { get; }

	/// <summary>
	///       Gets a value indicating whether this <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> can be personalized.
	///       </summary>
	/// <value>
	///   <c>true</c> if this setting is personalizable; otherwise, <c>false</c>.</value>
	public bool Personalizable { get; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> class with the <see cref="P:EPiServer.Shell.ViewComposition.Setting.Personalizable" /> property set to <c>false</c>.
	///       </summary>
	/// <param name="key">The key for the setting.</param>
	/// <param name="value">The value for the setting.</param>
	public Setting(string key, object value)
		: this(key, value, personalizable: false)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> class.
	///       </summary>
	/// <param name="key">The key for the setting.</param>
	/// <param name="personalizable">if set to <c>true</c> the value can be personalized.</param>
	public Setting(string key, bool personalizable)
		: this(key, null, personalizable)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Setting" /> class with information if the setting can be personalized.
	///       </summary>
	/// <param name="key">The key for the setting.</param>
	/// <param name="value">The value for the setting.</param>
	/// <param name="personalizable">if set to <c>true</c> the value can be personalized.</param>
	public Setting(string key, object value, bool personalizable)
	{
		if (value is Setting)
		{
			throw new ArgumentException("Value must not inherit from Setting", "value");
		}
		Key = key;
		Personalizable = personalizable;
		Value = value;
	}

	/// <summary>
	///       Implements the == operator.
	///       </summary>
	/// <param name="left">A Setting to compare.</param>
	/// <param name="right">A Setting to compare.</param>
	/// <returns>True if Key and Value of left and right are equal.</returns>
	public static bool operator ==(Setting left, Setting right)
	{
		if ((object)left == null && (object)right == null)
		{
			return true;
		}
		return left?.Equals(right) ?? right.Equals(left);
	}

	/// <summary>
	///       Implements the operator !=.
	///       </summary>
	/// <param name="left">A Setting to compare.</param>
	/// <param name="right">A Setting to compare.</param>
	/// <returns>True if key and Value of left and right differs.</returns>
	public static bool operator !=(Setting left, Setting right)
	{
		return !(left == right);
	}

	/// <summary>
	///       Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
	///       </summary>
	/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
	/// <returns>
	///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
	///       </returns>
	public override bool Equals(object obj)
	{
		Setting setting = obj as Setting;
		if (obj == null)
		{
			return false;
		}
		if (Key == setting.Key)
		{
			return Value == setting.Value;
		}
		return false;
	}

	/// <summary>
	///       Returns a hash code for this instance.
	///       </summary>
	/// <returns>
	///       A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
	///       </returns>
	public override int GetHashCode()
	{
		int num = 17;
		if (Key != null)
		{
			num = num * 23 + Key.GetHashCode();
		}
		if (Value != null)
		{
			num = num * 23 + Value.GetHashCode();
		}
		return num;
	}

	/// <summary>
	///       Copies this instance.
	///       </summary>
	/// <returns>A new instance with the values from the current settings object.</returns>
	public Setting Copy()
	{
		return MemberwiseClone() as Setting;
	}
}
