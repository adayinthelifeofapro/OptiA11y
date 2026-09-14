using System;

namespace EPiServer.Security;

/// <summary>
///       Actions that are supported by the provider.
///       </summary>
[Flags]
public enum ProviderActions
{
	/// <summary>
	///       Update action on provider
	///       </summary>
	Update = 1,
	/// <summary>
	///       Create action on provider
	///       </summary>
	Create = 2,
	/// <summary>
	///       Delete action on provider
	///       </summary>
	Delete = 4
}
