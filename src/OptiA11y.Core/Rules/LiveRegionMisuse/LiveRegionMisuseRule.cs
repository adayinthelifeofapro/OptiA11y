using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LiveRegionMisuse;

/// <summary>
/// Flags two situations: an <c>aria-live</c> value that isn't "off", "polite", or "assertive"
/// (invalid per spec, so assistive technology falls back to a default), and live-region markup
/// (<c>aria-live</c>, or a role like <c>status</c>/<c>alert</c>/<c>log</c>) applied to what looks
/// like short, static, one-time content rather than genuinely dynamic content. The parser can
/// only see the field's markup, not its runtime behaviour, so whether the region is genuinely
/// static is a judgement call - this always reports <see cref="Confidence.NeedsReview"/> to keep
/// both checks internally consistent.
/// </summary>
public sealed class LiveRegionMisuseRule : IContentRule
{
    private const int StaticContentLengthThreshold = 200;

    public string RuleId => "live-region-misuse";

    public string SuccessCriterion => "4.1.3";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var fragment in document.Get<LiveRegionFragment>())
        {
            if (fragment.HasInvalidPolitenessValue)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    fragment.Location,
                    $"This <{fragment.TagName}> has aria-live=\"{fragment.AriaLiveValue}\", which isn't a valid politeness value (\"off\", \"polite\", or \"assertive\"). Assistive technology will fall back to a default, which may not match the intended announcement behavior.");
            }

            if (fragment.TextLength > StaticContentLengthThreshold)
            {
                yield return new Finding(
                    RuleId,
                    SuccessCriterion,
                    Level,
                    Severity.Minor,
                    Confidence.NeedsReview,
                    fragment.Location,
                    $"This <{fragment.TagName}> is marked as a live region (aria-live or role=\"{fragment.Role}\") but contains a large block of text ({fragment.TextLength} characters), which looks more like static, one-time content than a genuinely dynamic status area. Verify this content actually updates at runtime.");
            }
        }
    }
}
