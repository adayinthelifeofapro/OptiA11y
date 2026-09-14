using System;

namespace EPiServer.Shell.Rest;

/// <summary>
///       Base class for client context.
///       </summary>
public abstract class ClientContextBase
{
	/// <summary>
	///       Gets or sets the uniform resource identifier to this selectable item.
	///       </summary>
	public Uri Uri { get; set; }

	/// <summary>
	///       Gest or sets the URL on which this item is displayed to users.
	///       </summary>
	public string PreviewUrl { get; set; }

	/// <summary>
	///       The data associated with this context. This data is provided by the context in order to reduce roundtrips.
	///       </summary>
	public object Data { get; set; }

	/// <summary>
	///       Gets a friendly name that can be displayed to the user.
	///       </summary>
	/// <value>The name.</value>
	public string Name { get; set; }

	/// <summary>
	///       Gets or sets the content class.
	///       </summary>
	/// <value>
	///       The content class.
	///       </value>
	public abstract string DataType { get; set; }

	/// <summary>
	///       Gets or sets the requested URI.
	///       </summary>
	/// <value>The requested URI.</value>
	public Uri RequestedUri { get; set; }

	/// <summary>
	///       Gets or sets the version agnostic URI.
	///       </summary>
	/// <value>The version agnostic URI.</value>
	public Uri VersionAgnosticUri { get; set; }

	/// <summary>
	///       Gets or sets the content Guid
	///       </summary>
	public string Key { get; set; }

	/// <summary>
	///       Gets or sets content work id
	///       </summary>
	public long Version { get; set; }
}
