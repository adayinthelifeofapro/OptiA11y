using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Builder;

namespace EPiServer.DependencyInjection;

/// <summary>
///       Extends <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> with extension methods to add authorization for shell module
///       </summary>
public static class AuthorizationExtensions
{
	/// <summary>
	///       Adds authorization policies with the specified names to the shell module.
	///       </summary>
	/// <typeparam name="TBuilder">The endpoint convention builder type.</typeparam>
	/// <param name="builder">The endpoint convention builder.</param>
	/// <param name="module">The shell module.</param>
	/// <returns>
	/// </returns>
	public static TBuilder RequireShellAuthorization<TBuilder>(this TBuilder builder, ShellModule module) where TBuilder : IEndpointConventionBuilder
	{
		if (!string.IsNullOrWhiteSpace(module.AuthorizationPolicy))
		{
			builder.RequireAuthorization(module.AuthorizationPolicy, "ReadOnlyShellModulePolicy");
		}
		return builder;
	}

	public static TBuilder RequireClientShellAuthorization<TBuilder>(this TBuilder builder, ShellModule module) where TBuilder : IEndpointConventionBuilder
	{
		builder.RequireAuthorization(module.ClientAuthorizationPolicy, "ReadOnlyShellModulePolicy");
		return builder;
	}
}
