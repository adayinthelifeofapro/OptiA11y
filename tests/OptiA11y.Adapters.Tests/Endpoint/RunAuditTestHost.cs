using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptiA11y.Cms.Features.RunAudit;
using OptiA11y.Cms.Infrastructure.ContentAdapter;
using OptiA11y.Core.Rules.AltTextQuality;
using OptiA11y.Core.Rules.HeadingStructure;
using OptiA11y.Core.Rules.LinkPurpose;
using OptiA11y.Core.Rules;

namespace OptiA11y.Adapters.Tests.Endpoint;

/// <summary>
/// Builds a minimal test host exposing only the RunAudit endpoint, wired against a fake
/// <see cref="IPaasContentLoader"/> so tests exercise the endpoint without a real CMS.
/// </summary>
internal static class RunAuditTestHost
{
    public static IHost Build(IPaasContentLoader loader)
    {
        var builder = new HostBuilder()
            .ConfigureWebHost(webHost =>
            {
                webHost.UseTestServer();
                webHost.ConfigureServices(services =>
                {
                    services.AddRouting();

                    services.AddSingleton<IContentRule, AltTextQualityRule>();
                    services.AddSingleton<IContentRule, HeadingStructureRule>();
                    services.AddSingleton<IContentRule, LinkPurposeRule>();
                    services.AddSingleton(sp => new RuleEngine(sp.GetServices<IContentRule>()));

                    services.AddSingleton(loader);
                    services.AddScoped<IContentAuditDocumentAdapter, PaasContentAdapter>();
                    services.AddScoped<RunAuditHandler>();
                });

                webHost.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapRunAuditEndpoint());
                });
            });

        return builder.Start();
    }
}
