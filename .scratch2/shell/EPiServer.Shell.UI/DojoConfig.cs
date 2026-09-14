using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using EPiServer.Framework.Serialization;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.UI;

/// <summary>
///       Configuration settings used when initiating the Dojo Toolkit on the client.
///       </summary>
/// <remarks>These settings will be serialized to JSON.</remarks>
[Serializable]
public class DojoConfig
{
	private readonly IObjectSerializerFactory _objectSerializerFactory;

	private static EventHandler _serializing;

	private readonly Dictionary<string, string> _paths = new Dictionary<string, string>();

	private readonly Dictionary<string, object> _has = new Dictionary<string, object>();

	/// <summary>
	///       Gets or sets whether we are in debug mode.
	///       </summary>
	public bool IsDebug { get; set; }

	/// <summary>
	///       Gets or sets the localization to use.
	///       </summary>
	public string Locale { get; set; }

	/// <summary>
	///       Gets or sets whether the config should be parsed on load.
	///       </summary>
	public bool ParseOnLoad { get; set; }

	/// <summary>
	///       Gets or sets whether the config should be async.
	///       </summary>
	public bool Async { get; set; }

	/// <summary>
	///       Gets or sets a collection of modules and their paths.
	///       </summary>
	public IDictionary<string, string> Paths => _paths;

	/// <summary>
	///       Gets a collection of aliases
	///       </summary>
	public IList<IEnumerable<string>> Aliases { get; private set; }

	/// <summary>
	///       Gets a collection of packages
	///       </summary>
	public IList<object> Packages { get; private set; }

	/// <summary>
	///       Gets if dojo should instrument the deferreds
	///       </summary>
	public bool DeferredInstrumentation => IsDebug;

	/// <summary>
	///       If DeferredInstrumtation is true report unhandled rejections
	///       </summary>
	public string UseDeferredInstrumentation
	{
		get
		{
			if (!DeferredInstrumentation)
			{
				return string.Empty;
			}
			return "report-unhandled-rejections";
		}
	}

	/// <summary>
	///        Gets current datetime of server
	///       </summary>
	public DateTime ServerTime => DateTime.UtcNow;

	/// <summary>
	///       Gets or sets whether to publish IO.
	///       </summary>
	public bool IoPublish { get; set; }

	/// <summary>
	///       Gets the has configuration options.
	///       </summary>
	public IDictionary<string, object> Has => _has;

	/// <summary>
	///       Exposes the BeforeSerializing event which is raised before the DojoConfig instance is serialized.
	///       </summary>
	public static event EventHandler Serializing
	{
		add
		{
			_serializing = (EventHandler)Delegate.Combine(_serializing, value);
		}
		remove
		{
			_serializing = (EventHandler)Delegate.Remove(_serializing, value);
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.UI.DojoConfig" /> class.
	///       </summary>
	public DojoConfig()
		: this(ServiceProviderExtensions.GetInstance<IObjectSerializerFactory>(ServiceLocator.Current))
	{
	}

	internal DojoConfig(IObjectSerializerFactory objectSerializerFactory)
	{
		_objectSerializerFactory = objectSerializerFactory;
		Aliases = new List<IEnumerable<string>>();
		Packages = new List<object>();
		Locale = ResolveLocale();
		IsDebug = false;
	}

	/// <summary>
	///       Called before the DojoConfig instance is serialized.
	///       </summary>
	private void OnSerializing()
	{
		_serializing?.Invoke(this, EventArgs.Empty);
	}

	/// <summary>
	///       Serializes this configuration to a JSON formatted string.
	///       </summary>
	/// <returns>A JSON formatted string of the Dojo configuration.</returns>
	public string Serialize()
	{
		OnSerializing();
		IObjectSerializer objectSerializer = _objectSerializerFactory.GetSerializer("application/json") ?? throw new ArgumentException("No serializer registered for content type application/json", "contentType");
		StringBuilder stringBuilder = new StringBuilder();
		using (StringWriter textWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
		{
			objectSerializer.Serialize(textWriter, this);
		}
		return stringBuilder.ToString();
	}

	private static string ResolveLocale()
	{
		CultureInfo cultureInfo = CultureInfo.CurrentUICulture;
		if (CultureInfo.CurrentUICulture.IsNeutralCulture)
		{
			cultureInfo = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentUICulture.Name);
		}
		string text = cultureInfo.Name.ToLowerInvariant();
		if (text.Equals("no", StringComparison.OrdinalIgnoreCase))
		{
			text = "nb";
		}
		return text;
	}
}
