using System;

namespace EPiServer.Shell;

/// <summary>
///       Options for the global header.
///       </summary>
public class GlobalHeaderOptions
{
	/// <summary>
	///       Gets or sets whether the common global header component should be used.
	///       </summary>
	public bool UseCommonComponent { get; set; }

	/// <summary>
	///       Gets or sets the current Instance ID.
	///       </summary>
	public string InstanceId { get; set; }

	/// <summary>
	///       Gets or sets the base url for the Global Header initialization script
	///       </summary>
	public string ScriptBaseUrl { get; set; } = "https://common.optimizely.com";

	/// <summary>
	///       Gets or sets the function that provides the access token.
	///       </summary>
	public Func<string> AccessToken { get; set; }

	/// <summary>
	///       Gets or sets the function that provides the logout URL.
	///       </summary>
	public Func<string> LogoutUrl { get; set; }
}
