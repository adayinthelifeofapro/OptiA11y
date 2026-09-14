using Microsoft.AspNetCore.Authorization;

namespace EPiServer.Shell.Modules.Internal;

public class ReadOnlyProtectedModulesRequirement : IAuthorizationRequirement
{
}
