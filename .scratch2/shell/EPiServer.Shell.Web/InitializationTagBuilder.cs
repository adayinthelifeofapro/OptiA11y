using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Web;
using EPiServer.Shell.Configuration;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.Shell.Web;

/// <summary>
///       Creates a shell script initiailzation html element.
///       </summary>
internal class InitializationTagBuilder : TagBuilder
{
	private readonly bool _async;

	private readonly IEnumerable<ShellModule> _shellModules;

	public InitializationTagBuilder(string defaultModuleArea, bool async, IEnumerable<ShellModule> shellModules)
		: base("script")
	{
		_async = async;
		_shellModules = shellModules;
		base.Attributes.Add("type", "text/javascript");
		if (async)
		{
			base.InnerHtml.SetHtmlContent("require([\"epi/routes\"], function(){");
		}
		base.InnerHtml.Append(string.Format(CultureInfo.InvariantCulture, "epi.routes.init(\"{0}\");", defaultModuleArea));
		AddModules();
	}

	private void AddModules()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ShellModule shellModule in _shellModules)
		{
			foreach (RouteDescription route in shellModule.Manifest.Routes)
			{
				Dictionary<string, string> dictionary = route.Defaults.ToDictionary((KeyValueElement e) => e.Key, (KeyValueElement e) => e.Value);
				if (!dictionary.ContainsKey("moduleArea"))
				{
					dictionary.Add("moduleArea", shellModule.Name);
				}
				using StringWriter stringWriter = new StringWriter();
				string value = JsonSerializer.Serialize(dictionary);
				stringWriter.Write(value);
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "epi.routes.registerRoute(\"{0}{1}\", {2});", shellModule.GetResolvedRouteBasePath(), route.Url, stringWriter.GetStringBuilder().ToString());
			}
		}
		base.InnerHtml.Append(stringBuilder.ToString());
		if (_async)
		{
			base.InnerHtml.Append("});");
		}
	}

	public override string ToString()
	{
		StringWriter stringWriter = new StringWriter();
		WriteTo(stringWriter, HtmlEncoder.Default);
		stringWriter.Flush();
		return HttpUtility.HtmlDecode(stringWriter.ToString());
	}
}
