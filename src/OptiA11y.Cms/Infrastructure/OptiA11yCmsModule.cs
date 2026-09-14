using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using OptiA11y.Cms.Extensions;
using OptiA11y.Cms.Infrastructure.ContentAdapter;

namespace OptiA11y.Cms.Infrastructure;

/// <summary>
/// Self-registers OptiA11y's slice-one services against the host CMS 13 site's DI container.
/// Being an <see cref="IConfigurableModule"/> means this happens automatically when the add-on
/// package is installed and the host's EPiServer initialization pipeline runs - no manual glue
/// code is required in the host solution. This also registers an <see cref="OptiA11yStartupFilter"/>
/// so the RunAudit endpoint is mapped automatically too.
/// </summary>
[InitializableModule]
[ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
public sealed class OptiA11yCmsModule : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.Services.AddOptiA11y();
        context.Services.AddScoped<IPaasContentLoader, EPiServerPaasContentLoader>();
        context.Services.AddTransient<IStartupFilter, OptiA11yStartupFilter>();
        context.Services.AddControllersWithViews();
    }

    public void Initialize(InitializationEngine context)
    {
    }

    public void Uninitialize(InitializationEngine context)
    {
    }
}
