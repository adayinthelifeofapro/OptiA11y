using System;
using System.Globalization;

namespace EPiServer.Shell.ViewComposition.Containers;

/// <summary>
///       Settings needed for children of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderContainer" /></summary>
[Serializable]
public class BorderSettingsDictionary : SettingsDictionary
{
	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderSettingsDictionary" /> class.
	///       </summary>
	/// <param name="region">The region.</param>
	/// <param name="settings">Optional settings.</param>
	public BorderSettingsDictionary(BorderContainerRegion region, params Setting[] settings)
		: this(region, resizable: false, null, null, null, settings)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderSettingsDictionary" /> class.
	///       </summary>
	/// <param name="region">The region.</param>
	/// <param name="resizable">if set to <c>true</c> [resizable].</param>
	/// <param name="size">The size.</param>
	/// <param name="settings">Optional settings.</param>
	public BorderSettingsDictionary(BorderContainerRegion region, bool resizable, int size, params Setting[] settings)
		: this(region, resizable, size, null, null, settings)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderSettingsDictionary" /> class.
	///       </summary>
	/// <param name="region">The region.</param>
	/// <param name="size">The size.</param>
	/// <param name="minSize">Size of the min.</param>
	/// <param name="maxSize">Size of the max.</param>
	/// <param name="settings">Optional settings.</param>
	public BorderSettingsDictionary(BorderContainerRegion region, int size, int? minSize, int? maxSize, params Setting[] settings)
		: this(region, resizable: true, size, minSize, maxSize, settings)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.Containers.BorderSettingsDictionary" /> class.
	///       </summary>
	/// <param name="region">The region.</param>
	/// <param name="resizable">if set to <c>true</c> the region will have a splitter and be resizable.</param>
	/// <param name="size">The size.</param>
	/// <param name="minSize">Min size of the region.</param>
	/// <param name="maxSize">Max size of the region.</param>
	/// <param name="settings">Optional settings.</param>
	public BorderSettingsDictionary(BorderContainerRegion region, bool resizable, int? size, int? minSize, int? maxSize, params Setting[] settings)
	{
		base["region"] = region;
		if (resizable)
		{
			base["splitter"] = resizable;
		}
		if (size.HasValue)
		{
			base["size"] = size;
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			base["style"] = string.Format(invariantCulture, "{0}:{1}px;", ((uint)region <= 1u) ? "height" : "width", size);
		}
		if (minSize.HasValue)
		{
			base["minSize"] = minSize;
		}
		if (maxSize.HasValue)
		{
			base["maxSize"] = maxSize;
		}
		MergeRange(settings);
	}
}
