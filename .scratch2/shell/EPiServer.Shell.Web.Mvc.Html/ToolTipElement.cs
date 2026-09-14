namespace EPiServer.Shell.Web.Mvc.Html;

/// <summary>
///       Conveys information about a row in a tool tip
///       </summary>
public class ToolTipElement
{
	/// <summary>
	///       The label portion
	///       </summary>
	public string Label { get; set; }

	/// <summary>
	///       The value portion
	///       </summary>
	public string Value { get; set; }

	/// <summary>
	///       Constructs an empty tool tip element
	///       </summary>
	public ToolTipElement()
	{
	}

	/// <summary>
	///       Constructs a tool tip with required values
	///       </summary>
	/// <param name="label">Used for <see cref="P:EPiServer.Shell.Web.Mvc.Html.ToolTipElement.Label" /></param>
	/// <param name="value">Used for <see cref="P:EPiServer.Shell.Web.Mvc.Html.ToolTipElement.Value" /></param>
	public ToolTipElement(string label, string value)
	{
		Label = label;
		Value = value;
	}

	/// <summary>
	///       Returns a <see cref="T:System.String" /> on the form [label]: [value].
	///       </summary>
	/// <returns>
	///       A <see cref="T:System.String" /> that represents the current <see cref="T:EPiServer.Shell.Web.Mvc.Html.ToolTipElement" />.
	///       </returns>
	public override string ToString()
	{
		return Label + ": " + Value;
	}
}
