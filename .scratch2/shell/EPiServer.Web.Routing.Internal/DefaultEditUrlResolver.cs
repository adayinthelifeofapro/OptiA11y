using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Applications;
using EPiServer.Core;
using EPiServer.Framework.Modules;
using Microsoft.Extensions.Options;

namespace EPiServer.Web.Routing.Internal;

internal class DefaultEditUrlResolver(IModuleResourceResolver resourcePathResolver, IApplicationResolver applicationResolver, IApplicationRepository applicationRepository, IOptions<UIOptions> uiOptions) : EditUrlResolver
{
	private const string DefaultModuleName = "CMS";

	protected override Url ResolveUrl(Application? application, ContentReference? contentLink, EditUrlArguments? editUrlArguments)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		if (editUrlArguments == null)
		{
			editUrlArguments = new EditUrlArguments();
		}
		UrlBuilder val = new UrlBuilder("");
		bool flag = editUrlArguments.ForceEditHost || editUrlArguments.ForceHost;
		if (application == null)
		{
			application = GetApplication(contentLink) ?? GetFallbackApplication(flag, editUrlArguments.Language);
		}
		Uri editHostUrl = GetEditHostUrl(application, flag, editUrlArguments.ForceHost, editUrlArguments.Language);
		if ((object)editHostUrl != null)
		{
			val = new UrlBuilder(editHostUrl);
		}
		else if (uiOptions.Value.EditUrl.IsAbsoluteUri)
		{
			val = new UrlBuilder(uiOptions.Value.EditUrl);
		}
		AssignPath(val, editUrlArguments);
		AssignLanguage(val, editUrlArguments.Language);
		AssignContent(val, contentLink);
		return new Url(val.Uri);
	}

	/// <summary>
	///       Resolve Application with according to this priority
	///       <list><item>1 - Take an application that its host type is Edit</item><item>2 - Otherwise take an application that its host match the language</item><item>3 - Otherwise take the first application</item></list></summary>
	private Application? GetFallbackApplication(bool forceEditSite, CultureInfo? language)
	{
		if (!forceEditSite)
		{
			return null;
		}
		IEnumerable<Application> source = applicationRepository.List();
		Application val = source.FirstOrDefault(delegate(Application a)
		{
			IRoutableApplication val2 = (IRoutableApplication)(object)((a is IRoutableApplication) ? a : null);
			return val2 != null && val2.Hosts.Any(delegate(ApplicationHost h)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				return (int)h.Type == 5;
			});
		});
		if (val == null && language != null)
		{
			val = source.FirstOrDefault(delegate(Application a)
			{
				IRoutableApplication val2 = (IRoutableApplication)(object)((a is IRoutableApplication) ? a : null);
				return val2 != null && val2.Hosts.Any(delegate(ApplicationHost h)
				{
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					return language.Equals(h.Locale) && !ApplicationHostExtensions.IsRedirect(h.Type);
				});
			});
		}
		return val ?? source.FirstOrDefault();
	}

	private Application? GetApplication(ContentReference? contentLink)
	{
		if (ContentReference.IsNullOrEmpty(contentLink))
		{
			return applicationResolver.GetByContext();
		}
		return applicationResolver.GetByContent(contentLink, true);
	}

	private Uri? GetEditHostUrl(Application? application, bool forceEditHost, bool forceHost, CultureInfo? language)
	{
		IRoutableApplication val = (IRoutableApplication)(object)((application is IRoutableApplication) ? application : null);
		if (val == null)
		{
			return null;
		}
		if (!forceEditHost)
		{
			Application byContext = applicationResolver.GetByContext();
			if (ApplicationExtensions.Equals(application, byContext, ApplicationComparer.NameOnly))
			{
				return null;
			}
		}
		ApplicationHost val2 = ApplicationHostExtensions.FirstOfType((IEnumerable<ApplicationHost>)val.Hosts, (ApplicationHostType)5);
		if (val2 != null)
		{
			return val2.Url;
		}
		if (forceHost)
		{
			if (language != null)
			{
				ApplicationHost primaryHost = IRoutableApplicationExtensions.GetPrimaryHost(val, language);
				if (primaryHost != null)
				{
					return primaryHost.Url;
				}
			}
			ApplicationHost primaryHost2 = IRoutableApplicationExtensions.GetPrimaryHost(val, (CultureInfo)null);
			if (primaryHost2 != null)
			{
				return primaryHost2.Url;
			}
			if ((object)val.Url == null)
			{
				return null;
			}
			return new Uri(val.Url.GetLeftPart(UriPartial.Authority));
		}
		return null;
	}

	private void AssignPath(UrlBuilder builder, EditUrlArguments editUrlArguments)
	{
		string text = resourcePathResolver.ResolvePath(editUrlArguments.ModuleName ?? "CMS", editUrlArguments.ModuleRelativePath);
		builder.Path = text.TrimStart('~');
	}

	private static void AssignLanguage(UrlBuilder builder, CultureInfo? language)
	{
		if (language != null && !language.Equals(CultureInfo.InvariantCulture))
		{
			builder.Query = "language=" + language.Name;
		}
	}

	private static void AssignContent(UrlBuilder builder, ContentReference? contentLink)
	{
		if (!ContentReference.IsNullOrEmpty(contentLink))
		{
			builder.Fragment = "context=epi.cms.contentdata:///" + (object)contentLink;
		}
	}
}
