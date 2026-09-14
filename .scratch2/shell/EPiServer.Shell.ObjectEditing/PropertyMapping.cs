namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Represents a mapping between property names.
///       </summary>
public struct PropertyMapping
{
	/// <summary>
	///       Gets or sets the name of the from property.
	///       </summary>
	public string From { get; set; }

	/// <summary>
	///       Gets or sets the name of the to property.
	///       </summary>
	public string To { get; set; }

	/// <inheritdoc />
	public static bool operator ==(PropertyMapping first, PropertyMapping second)
	{
		if (first.From == second.From)
		{
			return first.To == second.To;
		}
		return false;
	}

	/// <inheritdoc />
	public static bool operator !=(PropertyMapping first, PropertyMapping second)
	{
		return !(first == second);
	}

	/// <inheritdoc />
	public override readonly bool Equals(object obj)
	{
		if (!(obj is PropertyMapping))
		{
			return false;
		}
		return this == (PropertyMapping)obj;
	}

	/// <inheritdoc />
	public override readonly int GetHashCode()
	{
		int num = 0;
		if (From != null)
		{
			num += From.GetHashCode();
		}
		if (To != null)
		{
			num += 29 * To.GetHashCode();
		}
		return num;
	}

	/// <inheritdoc />
	public override readonly string ToString()
	{
		return (From ?? string.Empty) + "-" + (To ?? string.Empty);
	}
}
