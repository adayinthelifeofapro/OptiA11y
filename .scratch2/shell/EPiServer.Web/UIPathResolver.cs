using System;
using EPiServer.Applications;
using EPiServer.Framework.Modules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace EPiServer.Web;

/// <summary>
///       Contains helper methods to get or construct UI related paths or URIs
///       </summary>
public class UIPathResolver
{
	private readonly UIOptions _uiOptions;

	private readonly IModuleResourceResolver _moduleResourceResolver;

	private readonly IHttpContextAccessor _httpContextAccessor;

	private readonly IApplicationResolver _applicationResolver;

	private readonly IUriSupport _uriSupport;

	/// <summary>
	///       Static accessor to the singleton instance.
	///       </summary>
	/// <remarks>Recommendation is to take an instance of <see cref="T:EPiServer.Web.UIPathResolver" /> in constructor instead of
	///       using this accessor when possible. This makes the consuming class more testable.</remarks>
	[Obsolete("Inject or retrieve the UIPathResolver singleton instance directly from the service container.", true)]
	public static UIPathResolver Instance
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	///       Gets the absolute UI URL incuding authority.
	///       </summary>
	/// <value>The UI URL.</value>
	public virtual Uri AbsoluteUIUrl => CreateAbsoluteUri(InternalUIUrl);

	/// <summary>
	///       Gets the absolute util URL including authority.
	///       </summary>
	/// <value>The util URL.</value>
	public virtual Uri AbsoluteUtilUrl => CreateAbsoluteUri(InternalUtilUrl);

	private Url InternalUIUrl
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return new Url(_uiOptions.EditUrl);
		}
	}

	private Url InternalUtilUrl
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			return new Url(_uiOptions.UtilUrl);
		}
	}

	/// <summary>
	///       Creates a new instance of <see cref="T:EPiServer.Web.UIPathResolver" />.
	///       </summary>
	public UIPathResolver(UIOptions uiOptions, IModuleResourceResolver moduleResourceResolver, IHttpContextAccessor httpContextAccessor, IApplicationResolver applicationResolver, IUriSupport uriSupport)
	{
		_uiOptions = uiOptions;
		_moduleResourceResolver = moduleResourceResolver;
		_httpContextAccessor = httpContextAccessor;
		_applicationResolver = applicationResolver;
		_uriSupport = uriSupport;
	}

	/// <summary>
	///       Combines the relative path with the UI path.
	///       </summary>
	/// <param name="path">The path.</param>
	/// <returns>A root-relative path</returns>
	public virtual string CombineWithUI(string path)
	{
		return Combine(path, InternalUIUrl);
	}

	/// <summary>
	///       Combines specified path with UI url as relative or absolute.
	///       </summary>
	/// <param name="path">The path.</param>
	/// <returns>
	///       A string with the path to combine either as an absolute URL (with scheme, host etc) if the UIUrl is an absolute URL,
	///       or as a rooted, relative path (starting with /) if the UIUrl is not an absolute URL.
	///       </returns>
	public virtual string CombineWithUIRelativeOrAbsolute(string path)
	{
		if (InternalUIUrl.IsAbsoluteUri)
		{
			return CombineWithUIAbsolute(path);
		}
		return CombineWithUI(path);
	}

	/// <summary>
	///       Combines the relative path with the UI url, including scheme, host and port
	///       </summary>
	/// <param name="path">The path, will be resolved from UI if required.</param>
	/// <returns>A fully qualified URI starting with scheme, based on UIOptions settings</returns>
	public virtual string CombineWithUIAbsolute(string path)
	{
		return UriUtil.Combine(AbsoluteUIUrl.GetLeftPart(UriPartial.Authority), Combine(path, InternalUIUrl));
	}

	/// <summary>
	///       Resolves the path relative the Util path.
	///       </summary>
	/// <param name="path">The path.</param>
	/// <returns>A root-relative path</returns>
	public virtual string CombineWithUtil(string path)
	{
		return Combine(path, InternalUtilUrl);
	}

	/// <summary>
	///        Combines the relative path with the Util url, including scheme, host and port
	///       </summary>
	/// <param name="path">The path, will be resolved from Util if required.</param>
	/// <returns>A fully qualified URI starting with scheme, based on Web.Config settings</returns>
	public virtual string CombineWithUtilAbsolute(string path)
	{
		return UriUtil.Combine(AbsoluteUtilUrl.GetLeftPart(UriPartial.Authority), Combine(path, InternalUtilUrl));
	}

	/// <summary>
	///       Determines whether the path refers to a system path.
	///       </summary>
	/// <param name="urlPath">The path</param>
	/// <returns>
	///   <c>true</c> if the given path refers to a system directory; otherwise, <c>false</c>.
	///       </returns>
	public virtual bool IsSystemPath(string urlPath)
	{
		if (string.IsNullOrEmpty(urlPath))
		{
			return false;
		}
		if (_moduleResourceResolver.ProtectedRootPath.Length > 0 && urlPath.StartsWith(_moduleResourceResolver.ProtectedRootPath, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		string value = (InternalUtilUrl.IsAbsoluteUri ? InternalUtilUrl.Uri.AbsolutePath : VirtualPathUtilityEx.ToAbsolute(((object)InternalUtilUrl).ToString()));
		if (urlPath.StartsWith(value, StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		return false;
	}

	private string Combine(string path, Url baseUrl)
	{
		if (string.IsNullOrEmpty(path))
		{
			return VirtualPathUtilityEx.ToAbsolute(baseUrl.Path);
		}
		if (path[0] == '/')
		{
			return path;
		}
		if (path[0] == '~')
		{
			return VirtualPathUtilityEx.ToAbsolute(baseUrl.Path) + path;
		}
		return UriUtil.Combine(VirtualPathUtilityEx.ToAbsolute(baseUrl.Path), path);
	}

	private Uri CreateAbsoluteUri(Url url)
	{
		if (url.IsAbsoluteUri)
		{
			return url.Uri;
		}
		Application byContext = _applicationResolver.GetByContext();
		Application obj = ((byContext is IRoutableApplication) ? byContext : null);
		Uri uri = ((obj != null) ? ((IRoutableApplication)obj).Url : null);
		if ((object)uri != null)
		{
			return new Uri(UriUtil.Combine(uri.GetLeftPart(UriPartial.Authority), _uriSupport.ResolveUrlBySettings(url.OriginalString)));
		}
		return CreateAbsoluteUriByRequest(url, _httpContextAccessor?.HttpContext?.Request);
	}

	internal Uri CreateAbsoluteUriByRequest(Url url, HttpRequest context)
	{
		Uri uri = new Uri(url.Path, UriKind.RelativeOrAbsolute);
		if (!uri.IsAbsoluteUri && context != null)
		{
			uri = new UriBuilder(context.GetDisplayUrl())
			{
				Path = VirtualPathUtilityEx.ToAbsolute(url.Path)
			}.Uri;
		}
		return uri;
	}
}
