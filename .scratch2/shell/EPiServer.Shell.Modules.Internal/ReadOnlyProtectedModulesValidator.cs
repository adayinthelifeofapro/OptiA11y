using System;
using System.Globalization;
using EPiServer.Data;
using EPiServer.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules.Internal;

/// <summary>
///       Used to forbid or unavailable access when modules are stored in the "_Protected" folder and the application is in the ReadOnly mode.
///       </summary>
internal class ReadOnlyProtectedModulesValidator
{
	private readonly ILogger<ReadOnlyProtectedModulesValidator> _log;

	private readonly IDatabaseMode _databaseModeServer;

	private readonly IAccessReadOnlyProtectedModulesNotifier _notifier;

	private readonly ReadOnlyPageUrlResolver _readOnlyPageUrlResolver;

	private readonly UIPathResolver _uiPathResolver;

	/// <summary>
	///        Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.Internal.ReadOnlyProtectedModulesValidator" /> class.
	///       </summary>
	/// <param name="databaseModeServer">The database mode server</param>
	/// <param name="notifier">The AccessReadOnlyProtectedModules notifier</param>
	/// <param name="readOnlyPageUrlResolver">Resolver for ReadonlyPageUrl</param>
	/// <param name="uiPathResolver">Resolver for system pages</param>
	/// <param name="logger">The logger.</param>
	public ReadOnlyProtectedModulesValidator(IDatabaseMode databaseModeServer, IAccessReadOnlyProtectedModulesNotifier notifier, ReadOnlyPageUrlResolver readOnlyPageUrlResolver, UIPathResolver uiPathResolver, ILogger<ReadOnlyProtectedModulesValidator> logger)
	{
		ArgumentNullException.ThrowIfNull(databaseModeServer, "databaseModeServer");
		ArgumentNullException.ThrowIfNull(notifier, "notifier");
		_databaseModeServer = databaseModeServer;
		_notifier = notifier;
		_readOnlyPageUrlResolver = readOnlyPageUrlResolver;
		_uiPathResolver = uiPathResolver;
		_log = logger;
	}

	/// <summary>
	///        Raise the AccessReadOnlyProtectedModule event if the database mode is ReadOnly.
	///       </summary>
	/// <param name="context">HTTP context</param>
	public bool? Validate(HttpContext context)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		if ((int)_databaseModeServer.DatabaseMode == 1)
		{
			string text = VirtualPathUtilityEx.AppendTrailingSlash(ResolvePath(context));
			if (!string.IsNullOrWhiteSpace(text) && _uiPathResolver.IsSystemPath(text))
			{
				string readOnlyPageUrl = _readOnlyPageUrlResolver.GetReadOnlyPageUrl();
				if (text.Contains(readOnlyPageUrl, StringComparison.InvariantCultureIgnoreCase) || text.Contains("/Util/Login", StringComparison.InvariantCultureIgnoreCase))
				{
					return null;
				}
				string message = string.Format(CultureInfo.InvariantCulture, "The server is currently unable to handle the request to Path '" + text + "' because the application is in the ReadOnly mode.", default(ReadOnlySpan<object>));
				_log.ReadOnlyModeBlockingRequest(message);
				return _notifier.Notify(text);
			}
		}
		return null;
	}

	/// <summary>
	///       Resolve the path
	///       </summary>
	/// <param name="context">
	/// </param>
	/// <returns>
	/// </returns>
	protected virtual string ResolvePath(HttpContext context)
	{
		PathString? pathString = context.Request?.Path;
		if (!pathString.HasValue)
		{
			return null;
		}
		return pathString.GetValueOrDefault();
	}
}
