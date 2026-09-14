using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.TestHost;
using OptiA11y.Adapters.Tests.Paas;
using OptiA11y.Cms.Features.RunAudit;
using OptiA11y.Cms.Infrastructure.ContentAdapter;
using Xunit;

namespace OptiA11y.Adapters.Tests.Endpoint;

public sealed class RunAuditEndpointTests
{
    [Fact]
    public async Task GetAudit_ReturnsNotFound_WhenContentDoesNotExist()
    {
        using var host = RunAuditTestHost.Build(new FakePaasContentLoader(null));
        using var client = host.GetTestClient();

        var response = await client.GetAsync("/optia11y/audit/missing-content");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAudit_ReturnsFindings_ForContentWithAccessibilityIssues()
    {
        var node = new PaasContentNode(
            "page-1",
            BlockName: null,
            Properties: new[]
            {
                new PaasProperty(
                    "MainBody",
                    PaasPropertyKind.XhtmlString,
                    "<img src='a.jpg' /><a href='/x'>click here</a>",
                    Array.Empty<PaasContentNode>())
            });

        using var host = RunAuditTestHost.Build(new FakePaasContentLoader(node));
        using var client = host.GetTestClient();

        var response = await client.GetAsync("/optia11y/audit/page-1");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<RunAuditResult>();

        Assert.NotNull(result);
        Assert.Equal("page-1", result!.ContentReference);
        Assert.True(result.Findings.Count >= 2);
    }
}
