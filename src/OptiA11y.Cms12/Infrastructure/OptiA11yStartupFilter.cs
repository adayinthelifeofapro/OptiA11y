using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using OptiA11y.Cms12.Features.RunAudit;

namespace OptiA11y.Cms12.Infrastructure;

/// <summary>
/// Automatically maps the RunAudit endpoint into the host's ASP.NET Core pipeline, so hosts
/// do not need to call <see cref="RunAuditEndpoint.MapRunAuditEndpoint"/> themselves.
/// Registered by <see cref="OptiA11yCmsModule"/>, this runs as part of the standard
/// <c>IStartupFilter</c> pipeline that ASP.NET Core invokes when building the app.
/// </summary>
internal sealed class OptiA11yStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            next(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRunAuditEndpoint();
                endpoints.MapRunAuditPanelEndpoint();
            });
        };
    }
}
