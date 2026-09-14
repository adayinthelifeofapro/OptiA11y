using System.Threading.Tasks;
using EPiServer.Data;
using EPiServer.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules.Internal;

internal class ReadOnlyProtectedModulesAuthorizationHandler : AuthorizationHandler<ReadOnlyProtectedModulesRequirement>
{
	private readonly ReadOnlyProtectedModulesValidator _readOnlyProtectedModulesValidator;

	private readonly IHttpContextAccessor _httpContextAccessor;

	public ReadOnlyProtectedModulesAuthorizationHandler(IDatabaseMode databaseMode, IAccessReadOnlyProtectedModulesNotifier accessReadOnlyProtectedModulesNotifier, IHttpContextAccessor httpContextAccessor, ReadOnlyPageUrlResolver readOnlyPageUrlResolver, UIPathResolver uiPathResolver, ILogger<ReadOnlyProtectedModulesValidator> logger)
	{
		_readOnlyProtectedModulesValidator = new ReadOnlyProtectedModulesValidator(databaseMode, accessReadOnlyProtectedModulesNotifier, readOnlyPageUrlResolver, uiPathResolver, logger);
		_httpContextAccessor = httpContextAccessor;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReadOnlyProtectedModulesRequirement requirement)
	{
		if (_httpContextAccessor?.HttpContext == null || !_readOnlyProtectedModulesValidator.Validate(_httpContextAccessor.HttpContext).HasValue)
		{
			context.Succeed(requirement);
		}
		return Task.CompletedTask;
	}
}
