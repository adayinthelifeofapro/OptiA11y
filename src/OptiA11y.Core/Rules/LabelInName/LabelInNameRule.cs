using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LabelInName;

/// <summary>
/// Flags an element whose visible label text is not fully contained within its computed
/// accessible name (WCAG 2.5.3). <see cref="LabelInNameFragment"/> is only ever produced by the
/// rendered-style enrichment slice from a direct text comparison, so a mismatch is a structural
/// fact, hence <see cref="Confidence.Fail"/>.
/// </summary>
public sealed class LabelInNameRule : IContentRule
{
    public string RuleId => "label-in-name";

    public string SuccessCriterion => "2.5.3";

    public WcagLevel Level => WcagLevel.A;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var element in document.Get<LabelInNameFragment>())
        {
            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Major,
                Confidence.Fail,
                element.Location,
                $"{element.ElementDescription}'s visible label (\"{element.VisibleLabelText}\") is not contained in its accessible name (\"{element.AccessibleName}\"). Speech-input users who speak the visible label may fail to activate this control - make the accessible name include the visible text.");
        }
    }
}
