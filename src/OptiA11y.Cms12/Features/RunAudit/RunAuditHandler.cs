using OptiA11y.Core.Model;
using OptiA11y.Core.Rules;
using OptiA11y.Rendering;

namespace OptiA11y.Cms12.Features.RunAudit;

/// <summary>
/// Coordinates building an <see cref="AuditDocument"/> for the requested content and running
/// it through the <see cref="RuleEngine"/>. Results are returned to the caller only — this
/// slice deliberately holds no persistence.
/// </summary>
public sealed class RunAuditHandler
{
    private const string RenderedStylesPropertyName = "__RenderedStyles";

    private readonly IContentAuditDocumentAdapter _adapter;
    private readonly RuleEngine _ruleEngine;
    private readonly IEditorLinkResolver? _editorLinkResolver;
    private readonly IRenderedStyleProvider? _renderedStyleProvider;
    private readonly IContentPreviewUrlResolver? _previewUrlResolver;

    public RunAuditHandler(
        IContentAuditDocumentAdapter adapter,
        RuleEngine ruleEngine,
        IEditorLinkResolver? editorLinkResolver = null,
        IRenderedStyleProvider? renderedStyleProvider = null,
        IContentPreviewUrlResolver? previewUrlResolver = null)
    {
        _adapter = adapter;
        _ruleEngine = ruleEngine;
        _editorLinkResolver = editorLinkResolver;
        _renderedStyleProvider = renderedStyleProvider;
        _previewUrlResolver = previewUrlResolver;
    }

    public async Task<RunAuditResult?> HandleAsync(RunAuditCommand command, CancellationToken cancellationToken = default)
    {
        var document = await _adapter.BuildAsync(command.ContentReference, cancellationToken);
        if (document is null)
        {
            return null;
        }

        document = await EnrichWithRenderedStylesAsync(document, cancellationToken);

        var findings = _ruleEngine.Audit(document);

        var editorLinks = new Dictionary<string, string>();
        if (_editorLinkResolver is not null)
        {
            foreach (var finding in findings)
            {
                var key = finding.Location.ToPathString();
                if (editorLinks.ContainsKey(key))
                {
                    continue;
                }

                var link = _editorLinkResolver.ResolveEditorLink(finding.Location);
                if (link is not null)
                {
                    editorLinks[key] = link;
                }
            }
        }

        return new RunAuditResult(command.ContentReference, findings, editorLinks);
    }

    /// <summary>
    /// Best-effort enrichment: if both a rendered-style provider and a resolvable preview URL are
    /// available, renders the page and appends contrast/text-style fragments built from real
    /// computed CSS. If either is missing, or rendering fails, returns the document unchanged so
    /// callers always fall back to the inline-style-only fragments from <c>HtmlFragmentParser</c>.
    /// </summary>
    private async Task<AuditDocument> EnrichWithRenderedStylesAsync(AuditDocument document, CancellationToken cancellationToken)
    {
        if (_renderedStyleProvider is null || _previewUrlResolver is null)
        {
            return document;
        }

        var previewUrl = _previewUrlResolver.ResolvePreviewUrl(document.ContentReference);
        if (previewUrl is null)
        {
            return document;
        }

        var diagnostics = await _renderedStyleProvider.CaptureAsync(previewUrl, cancellationToken);
        if (diagnostics.IsEmpty)
        {
            return document;
        }

        var location = SourceLocation.OnProperty(document.ContentReference, RenderedStylesPropertyName);
        var renderedFragments = RenderedStyleFragmentBuilder.Build(diagnostics, location);

        var combinedFragments = document.Fragments.Concat(renderedFragments).ToList();
        return new AuditDocument(document.ContentReference, combinedFragments);
    }
}

