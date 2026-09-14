using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EPiServer.Shell.ObjectEditing.EditorDescriptors;

/// <summary>
///       Base class for all numeric type's editor descriptor
///       </summary>
public abstract class NumericEditorDescriptor : EditorDescriptor
{
	private const string constraintKey = "constraints";

	private const string maxKey = "Max";

	private const string minKey = "Min";

	private const string placesKey = "Places";

	private const string exponentKey = "Exponent";

	private readonly string _places;

	private readonly bool _isExponent;

	private object _defaultValue;

	/// <inheritdoc />
	public override object DefaultValue
	{
		get
		{
			return _defaultValue;
		}
		set
		{
			_defaultValue = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.NumericEditorDescriptor" /> class.
	///       </summary>
	protected NumericEditorDescriptor(object min, object max, int significantDigits)
	{
		base.ClientEditingClass = "epi-cms/contentediting/editors/RangeNumberEditor";
		_defaultValue = 0;
		_isExponent = significantDigits != 0;
		_places = string.Format(CultureInfo.InvariantCulture, "0,{0}", Math.Min(significantDigits, 20));
		base.EditorConfiguration["constraints"] = new Dictionary<string, object>
		{
			{ "Max", max },
			{ "Min", min },
			{ "Places", _places },
			{ "Exponent", _isExponent }
		};
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.EditorDescriptors.NumericEditorDescriptor" /> class.
	///       </summary>
	protected NumericEditorDescriptor(object min, object max)
		: this(min, max, 0)
	{
	}

	/// <inheritdoc />
	public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
	{
		base.ModifyMetadata(metadata, attributes);
		RangeAttribute rangeAttribute = attributes.FirstOfType<RangeAttribute>();
		if (rangeAttribute != null && metadata.EditorConfiguration.TryGetValue("constraints", out var value) && value is Dictionary<string, object> dictionary)
		{
			Dictionary<string, object> value2 = new Dictionary<string, object>(dictionary)
			{
				["Max"] = rangeAttribute.Maximum,
				["Min"] = rangeAttribute.Minimum
			};
			metadata.EditorConfiguration["constraints"] = value2;
		}
	}
}
