using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LinkTextLanguage;

/// <summary>
/// Flags link text that appears to be written in a different writing system (Cyrillic, Greek,
/// Hebrew, Arabic, CJK, Hangul, Thai, Devanagari) than the link's declared <c>lang</c>, with no
/// override marking the change. As with body text (see <c>language-of-parts</c>), script
/// detection is a strong signal but not proof of a language change - a proper noun or brand
/// name in another script doesn't necessarily need a lang override - so this always reports
/// <see cref="Confidence.NeedsReview"/>.
/// </summary>
public sealed class LinkTextLanguageRule : IContentRule
{
    private static readonly Regex NonLatinScriptRun = new(
        @"[Ͱ-ϿЀ-ӿ֐-׿؀-ۿऀ-ॿ฀-๿぀-ヿ一-鿿가-힯]{3,}",
        RegexOptions.Compiled);

    public string RuleId => "link-text-language";

    public string SuccessCriterion => "3.1.2";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var link in document.Get<LinkFragment>())
        {
            if (link.LanguageCode is not null || string.IsNullOrWhiteSpace(link.Text))
            {
                continue;
            }

            var match = NonLatinScriptRun.Match(link.Text);
            if (!match.Success)
            {
                continue;
            }

            yield return new Finding(
                RuleId,
                SuccessCriterion,
                Level,
                Severity.Minor,
                Confidence.NeedsReview,
                link.Location,
                $"The link text \"{link.Text}\" looks like it's in a different writing system than the surrounding declared language, with no lang override. If this is genuinely a different language, wrap the link in an element with the correct lang attribute.");
        }
    }
}
