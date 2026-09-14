using System.Collections.Specialized;
using EPiServer.Shell.Services.Rest;

namespace EPiServer.Shell.Rest;

/// <summary>
///       The interface which carries parameters to a query.
///       </summary>
public interface IQueryParameters
{
	/// <summary>
	///       All parameters (unparsed) from the original request.
	///       </summary>
	NameValueCollection AllParameters { get; }

	/// <summary>
	///       Requested range.
	///       </summary>
	ItemRange Range { get; set; }
}
