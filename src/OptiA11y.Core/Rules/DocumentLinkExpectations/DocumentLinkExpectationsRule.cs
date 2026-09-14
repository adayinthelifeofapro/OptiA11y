using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.DocumentLinkExpectations;

/// <summary>
/// Flags links to downloadable documents (PDF, DOCX, XLSX, etc.) whose visible text gives no
/// warning that activating it leaves the page and opens a document, which the current focus may
/// then move outside of the assistive-technology-friendly page context. Whether the visible text
/// "conveys the format" is a judgement call, so this is always NeedsReview.
/// </summary>
public sealed class DocumentLinkExpectationsRule : IContentRule
{
    private static readonly string[] FormatHintKeywords =
    {
        "pdf", "doc", "word", "excel", "xls", "spreadsheet", "powerpoint", "ppt", "slides",
        "download", "document", "file"
    };

    public string RuleId => "document-link-expectations";

    public string SuccessCriterion => "2.4.4";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var link in document.Get<LinkFragment>())
        {
            if (!link.IsDocumentLink)
            {
                continue;
            }

            var text = link.Text.Trim();
            if (text.Length == 0)
            {
                continue;
            }

            var mentionsFormat = FormatHintKeywords.Any(keyword => text.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            if (!mentionsFormat)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    link.Location,
                    $"The link text \"{link.Text}\" points to a downloadable document but doesn't say so. Consider naming the file type (e.g. \"Annual report (PDF)\") so users know what will happen before they activate it.");
            }
        }
    }
}
