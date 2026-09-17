using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.MetaRefresh;

/// <summary>
/// Flags a <c>&lt;meta http-equiv="refresh"&gt;</c> tag found in content markup. An automatic
/// timed refresh or redirect gives users no way to stop or extend it, and screen reader/keyboard
/// users may be moved away from a page mid-interaction. Whether the tag is present at all is a
/// deterministic structural fact, so this reports <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class MetaRefreshRule : IContentRule
{
    public string RuleId => "meta-refresh";

    public string SuccessCriterion => "2.2.1";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var metaRefresh in document.Get<MetaRefreshFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                metaRefresh.Location,
                $"This content includes a <meta http-equiv=\"refresh\" content=\"{metaRefresh.Content}\"> tag, which automatically refreshes or redirects the page with no way for the user to stop or extend it. Remove it and use a server-side redirect or a user-triggered link instead.");
        }
    }
}
