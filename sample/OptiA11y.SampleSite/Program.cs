using OptiA11y.Cms.Extensions;
using OptiA11y.Cms.Features.RunAudit;
using OptiA11y.Cms.Infrastructure.ContentAdapter;
using OptiA11y.SampleSite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptiA11y();
builder.Services.AddSingleton<IPaasContentLoader, SeededPaasContentLoader>();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/optia11y/audit/page-home"));
app.MapRunAuditEndpoint();

app.Run();
