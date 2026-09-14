using System;
using System.Globalization;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Defines a component that will load a page inside an iframe with a defined source location.
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public class IFrameComponentAttribute : ComponentAttribute
{
	/// <summary>
	///       Gets the type of Widget that is used for the IFrameComponent
	///       </summary>
	public override string WidgetType
	{
		get
		{
			return "epi/shell/component/IFrameContextComponent";
		}
		set
		{
		}
	}

	/// <summary>
	///       Gets or sets the source URL for the iframe to load.
	///       </summary>
	/// <value>The source URL.</value>
	public virtual string Url { get; set; }

	/// <summary>
	///       Gets or sets if the iFrame should be reloaded when the context is changed. The default value is true.
	///       </summary>
	/// <value>If the iFrame should be reloaded when the context is changed.</value>
	public bool ReloadOnContextChange { get; set; }

	/// <summary>
	///       Gets or sets if the iFrame should keep the current url when the context is changed and the iFrame is reloaded.
	///       The default value is false.
	///       </summary>
	/// <value>If the iFrame should keep the url when context is changed.</value>
	public bool KeepUrlOnContextChange { get; set; }

	/// <summary>
	///       The minimum height of the iFrame. This is set as a stylesheet attribute. The default value is 100.
	///       </summary>
	/// <value>The minimum height of the iFrame.</value>
	public int MinHeight { get; set; }

	/// <summary>
	///       The maximum height of the iFrame. This is set as a stylesheet attribute. The default value is 500.
	///       </summary>
	/// <value>The maximum height of the iFrame.</value>
	public int MaxHeight { get; set; }

	/// <summary>
	///       If a wrapper is used to encapsulate a UserControl, this property can be set to access its URI used when loading the control. The default value is null.
	///       </summary>
	/// <value>The URI to the ascx of the control.</value>
	protected virtual string ControlUri { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.IFrameComponentAttribute" /> class.
	///       </summary>
	public IFrameComponentAttribute()
	{
		ReloadOnContextChange = true;
		KeepUrlOnContextChange = false;
		MinHeight = 100;
		MaxHeight = 500;
	}

	/// <summary>
	///       Creates the component definition from the settings provided in the attribute properties.
	///       </summary>
	/// <returns>
	///       A new instance of a <see cref="T:EPiServer.Shell.ViewComposition.DefaultComponentDefinition" />.
	///       </returns>
	public override IComponentDefinition CreateComponentDefinition(Type attributedType)
	{
		Settings["urlTemplate"] = (new Uri(Url, UriKind.RelativeOrAbsolute).IsAbsoluteUri ? Url : Paths.ToResource(attributedType.Assembly, Url));
		Settings["reloadOnContextChange"] = ReloadOnContextChange;
		Settings["keepUrlOnContextChange"] = KeepUrlOnContextChange;
		Settings["minHeight"] = MinHeight;
		Settings["maxHeight"] = MaxHeight;
		if (!string.IsNullOrEmpty(ControlUri))
		{
			Settings["controlUri"] = ControlUri;
			string arg = (((string)Settings["urlTemplate"]).Contains('?') ? "&" : "?");
			Settings["urlTemplate"] = string.Format(CultureInfo.InvariantCulture, "{0}{1}fn={2}", Settings["urlTemplate"], arg, attributedType.FullName);
		}
		return base.CreateComponentDefinition(attributedType);
	}
}
