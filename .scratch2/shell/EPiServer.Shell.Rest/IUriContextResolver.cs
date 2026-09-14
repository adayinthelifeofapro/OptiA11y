using System;
using System.Threading;
using System.Threading.Tasks;

namespace EPiServer.Shell.Rest;

/// <summary>
///       Resolves context given an instance URI.
///       </summary>
public interface IUriContextResolver
{
	/// <summary>
	///       Gets the name for this type of data.
	///       </summary>
	string Name { get; }

	/// <summary>
	///       Resolves the context for a given URI.
	///       </summary>
	/// <param name="uri">The URI uniquely identifying a specific data.</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Returns the context resolution result containing the context and potential errors.</returns>
	Task<ContextResolutionResult> TryResolveUriAsync(Uri uri, CancellationToken cancellationToken = default(CancellationToken));
}
