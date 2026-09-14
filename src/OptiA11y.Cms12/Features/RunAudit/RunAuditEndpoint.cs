using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OptiA11y.Cms12.Features.RunAudit;

/// <summary>
/// Maps the standalone RunAudit HTTP endpoint. Deliberately a plain endpoint rather than a
/// React in-context panel, since the CMS 13 editor extensibility surface for in-context panels
/// is unverified — a standalone view is the safe route for slice one either way.
/// </summary>
public static class RunAuditEndpoint
{
    public const string RoutePattern = "/optia11y/audit/{contentReference}";

    public static IEndpointRouteBuilder MapRunAuditEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePattern, async (string contentReference, RunAuditHandler handler, CancellationToken cancellationToken) =>
        {
            var result = await handler.HandleAsync(new RunAuditCommand(contentReference), cancellationToken);

            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .WithName("OptiA11yRunAudit");

        return endpoints;
    }
}
