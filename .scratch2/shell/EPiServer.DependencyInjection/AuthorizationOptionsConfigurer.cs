using System;
using EPiServer.Authorization;
using EPiServer.Shell.Modules.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EPiServer.DependencyInjection;

internal class AuthorizationOptionsConfigurer : IConfigureOptions<AuthorizationOptions>
{
	private readonly IOptions<CmsPolicyOptions> _cmsPolicyPolicyOptions;

	public AuthorizationOptionsConfigurer(IOptions<CmsPolicyOptions> cmsPolicyOptions)
	{
		_cmsPolicyPolicyOptions = cmsPolicyOptions;
	}

	public void Configure(AuthorizationOptions options)
	{
		AuthorizationOptionsExtensions.TryAddPolicy(options, "episerver:defaultshellmodule", (Action<AuthorizationPolicyBuilder>)delegate(AuthorizationPolicyBuilder policy)
		{
			policy.RequireRole(_cmsPolicyPolicyOptions.Value.DefaultShellModuleRoles);
		});
		AuthorizationOptionsExtensions.TryAddPolicy(options, "episerver:cmsedit", (Action<AuthorizationPolicyBuilder>)delegate(AuthorizationPolicyBuilder policy)
		{
			policy.RequireRole(_cmsPolicyPolicyOptions.Value.EditRoles);
		});
		AuthorizationOptionsExtensions.TryAddPolicy(options, "ReadOnlyShellModulePolicy", (Action<AuthorizationPolicyBuilder>)delegate(AuthorizationPolicyBuilder policy)
		{
			policy.AddRequirements(new ReadOnlyProtectedModulesRequirement());
		});
	}
}
