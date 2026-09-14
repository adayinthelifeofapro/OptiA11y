using System;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.Framework;

namespace EPiServer.Shell.Rest;

/// <summary>
///       Resolves a context given a friendly URL.
///       </summary>
public interface IUrlContextResolver : ISortable
{
	/// <summary>
	///       Resolves the context for a given preview URL.
	///       </summary>
	/// <param name="url">The URL on which the given data is displayed to users.</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>Returns the context resolution result containing the context and potential errors.</returns>
	Task<ContextResolutionResult> TryResolveUrlAsync(Uri url, CancellationToken cancellationToken = default(CancellationToken));
}
