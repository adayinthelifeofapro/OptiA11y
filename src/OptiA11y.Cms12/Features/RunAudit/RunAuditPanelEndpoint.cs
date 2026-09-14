using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace OptiA11y.Cms12.Features.RunAudit;

/// <summary>
/// Serves the accessibility audit report as an HTML page, rendered inside the CMS 13 editor's
/// "assets" pane iframe registered by
/// <see cref="OptiA11y.Cms12.Infrastructure.Navigation.OptiA11yAuditPanelPlugin"/>.
///
/// The panel widget (<c>epi/shell/component/IFrameContextComponent</c>) appends the current
/// editor context as an <c>id</c> query string parameter (confirmed from the CMS shell's own
/// client-resource source: <c>epi/shell/socket/MessageHub.js</c> compares <c>context.id</c>
/// directly against <c>contentLink</c> values), and reloads the iframe whenever that context
/// changes, so this endpoint always reflects whichever page or block is currently open. It
/// renders the existing <c>RunAudit.cshtml</c> Razor view directly via the view engine, so the
/// same report markup used for direct/deep links is reused here without requiring the host to
/// register full MVC controllers.
/// </summary>
public static class RunAuditPanelEndpoint
{
    public const string RoutePattern = "/optia11y/audit/panel";
    private const string ViewPath = "/Features/RunAudit/Views/RunAudit.cshtml";

    public static IEndpointRouteBuilder MapRunAuditPanelEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePattern, async (
            HttpContext httpContext,
            RunAuditHandler handler,
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            CancellationToken cancellationToken) =>
        {
            // The IFrameContextComponent widget sends the current context's content reference
            // as "id". "contentLink" is also accepted so the panel can be linked to directly
            // (e.g. for manual testing) using the same parameter name as the standalone endpoint.
            var contentReference = httpContext.Request.Query["id"].FirstOrDefault()
                ?? httpContext.Request.Query["contentLink"].FirstOrDefault();

            if (string.IsNullOrEmpty(contentReference))
            {
                return Results.Content(
                    "<p>Open a page or block in the editor to see its accessibility audit.</p>",
                    "text/html");
            }

            var result = await handler.HandleAsync(new RunAuditCommand(contentReference), cancellationToken);

            if (result is null)
            {
                return Results.Content(
                    "<p>No accessibility audit is available for this content.</p>",
                    "text/html");
            }

            var html = await RenderViewAsync(viewEngine, tempDataProvider, httpContext, ViewPath, result);
            return Results.Content(html, "text/html");
        })
        .WithName("OptiA11yRunAuditPanel");

        return endpoints;
    }

    private static async Task<string> RenderViewAsync(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        HttpContext httpContext,
        string viewPath,
        RunAuditResult model)
    {
        var actionContext = new Microsoft.AspNetCore.Mvc.ActionContext(
            httpContext,
            httpContext.GetRouteData(),
            new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

        var viewResult = viewEngine.GetView(executingFilePath: null, viewPath, isMainPage: true);
        if (!viewResult.Success)
        {
            viewResult = viewEngine.FindView(actionContext, viewPath, isMainPage: true);
        }

        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"Could not find view '{viewPath}'.");
        }

        await using var writer = new StringWriter();

        var viewDataDictionary = new ViewDataDictionary<RunAuditResult>(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary())
        {
            Model = model,
        };

        var tempData = new TempDataDictionary(httpContext, tempDataProvider);

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDataDictionary,
            tempData,
            writer,
            new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);

        return writer.ToString();
    }
}
