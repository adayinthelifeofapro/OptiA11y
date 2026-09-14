using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.PageTitle;

/// <summary>
/// Evaluates the audited content item's own name/title (WCAG 2.4.2, Page Titled). A completely
/// missing/empty name is a deterministic structural fact, so that reports
/// <see cref="Confidence.Fail"/>. A generic placeholder name ("untitled", "new page", "home") is
/// a judgement call about editorial intent, so that reports <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class PageTitleRule : IContentRule
{
    private static readonly string[] PlaceholderNames =
    {
        "untitled", "new page", "new content", "page", "home page", "default", "test"
    };

    public string RuleId => "page-title";

    public string SuccessCriterion => "2.4.2";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var metadata in document.Get<PageMetadataFragment>())
        {
            var name = metadata.DisplayName?.Trim();

            if (string.IsNullOrEmpty(name))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Major,
                    Confidence.Fail,
                    metadata.Location,
                    "This content item has no name/title at all. A descriptive page title helps every user - and especially screen reader and browser-tab users - identify and navigate between pages.");
                continue;
            }

            if (PlaceholderNames.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    metadata.Location,
                    $"This content item's name, \"{metadata.DisplayName}\", looks like a placeholder rather than a descriptive title. Confirm it accurately identifies the page's content and purpose.");
            }
        }
    }
}
