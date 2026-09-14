using System.Text.RegularExpressions;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Rules.LanguageOfParts;

/// <summary>
/// Flags a passage of text that appears to switch to a different writing system (Cyrillic,
/// Greek, Hebrew, Arabic, CJK, Hangul, Thai, Devanagari) without a <c>lang</c> attribute on that
/// passage, so assistive technology has no way to pick the correct pronunciation. Script
/// detection is a strong signal but not proof of a language change (a proper noun or brand name
/// in another script isn't necessarily "content in another language"), so this is NeedsReview.
/// </summary>
public sealed class LanguageOfPartsRule : IContentRule
{
    private static readonly Regex NonLatinScriptRun = new(
        @"[Ͱ-ϿЀ-ӿ֐-׿؀-ۿऀ-ॿ฀-๿぀-ヿ一-鿿가-힯]{3,}",
        RegexOptions.Compiled);

    public string RuleId => "language-of-parts";

    public string SuccessCriterion => "3.1.2";

    public WcagLevel Level => WcagLevel.AA;

    public IEnumerable<Finding> Evaluate(AuditDocument document)
    {
        foreach (var text in document.Get<TextFragment>())
        {
            if (text.LanguageCode is not null)
            {
                continue;
            }

            var match = NonLatinScriptRun.Match(text.Text);
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
                text.Location,
                $"This text contains a passage in what looks like a different writing system (\"{match.Value}\") with no lang attribute marking it. If this is genuinely a different language, wrap it in an element with the correct lang attribute so assistive technology pronounces it correctly.");
        }
    }
}
