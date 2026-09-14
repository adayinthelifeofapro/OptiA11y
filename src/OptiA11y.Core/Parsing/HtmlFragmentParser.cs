using HtmlAgilityPack;
using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;

namespace OptiA11y.Core.Parsing;

/// <summary>
/// Parses a fragment of HTML (as found in a rich-text property, on either PaaS or SaaS) into
/// content fragments. Shared by every adapter so image/link/heading extraction is identical
/// regardless of where the HTML came from. This depends only on HtmlAgilityPack, not on any
/// CMS package, so it can live in Core without breaking the no-CMS-dependency constraint.
/// </summary>
public static class HtmlFragmentParser
{
    private static readonly string[] DocumentExtensions =
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx"
    };

    private static readonly HashSet<string> DeprecatedElementNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "blink", "marquee", "font", "center", "big", "strike", "tt", "acronym", "frame", "frameset", "applet"
    };

    private static readonly HashSet<string> NativelyInteractiveTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "button", "input", "select", "textarea"
    };

    private static readonly System.Text.RegularExpressions.Regex BulletLineRegex = new(
        @"^\s*([-*•▪◦]|\(?\d+[\.\)])\s+",
        System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Multiline);

    public static IEnumerable<ContentFragment> Parse(string html, SourceLocation baseLocation)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            yield break;
        }

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var ordinal = 0;

        foreach (var node in doc.DocumentNode.DescendantsAndSelf())
        {
            switch (node.Name)
            {
                case "img":
                    yield return BuildImageFragment(node, baseLocation, ordinal++);
                    break;

                case "a":
                    yield return BuildLinkFragment(node, baseLocation, ordinal++);
                    break;

                case "h1" or "h2" or "h3" or "h4" or "h5" or "h6":
                    yield return BuildHeadingFragment(node, baseLocation, ordinal++);
                    break;

                case "table":
                    yield return BuildTableFragment(node, baseLocation, ordinal++);
                    break;

                case "video" or "audio":
                    yield return BuildMediaFragment(node, baseLocation, ordinal++);
                    break;

                case "input" or "select" or "textarea":
                    yield return IsButtonLikeInput(node)
                        ? BuildButtonFragment(node, baseLocation, ordinal++, $"input[{node.GetAttributeValue("type", string.Empty)}]")
                        : BuildFormFieldFragment(node, baseLocation, ordinal++);
                    break;

                case "button":
                    yield return BuildButtonFragment(node, baseLocation, ordinal++, "button");
                    break;

                case "iframe":
                    yield return BuildIframeFragment(node, baseLocation, ordinal++);
                    break;

                case "ul" or "ol":
                    yield return BuildListStructureFragment(node, baseLocation, ordinal++);
                    break;

                case "p" or "div" or "span":
                    var fakeList = TryBuildFakeListFragment(node, baseLocation, ordinal);
                    if (fakeList is not null)
                    {
                        ordinal++;
                        yield return fakeList;
                    }
                    break;

                case "html":
                    var langFragment = TryBuildLanguageAttributeFragment(node, baseLocation, ordinal);
                    if (langFragment is not null)
                    {
                        ordinal++;
                        yield return langFragment;
                    }
                    break;
            }

            if (DeprecatedElementNames.Contains(node.Name))
            {
                yield return new DeprecatedElementFragment(baseLocation with { Ordinal = ordinal++ }, node.Name);
            }

            if (node.NodeType == HtmlNodeType.Element && node != doc.DocumentNode)
            {
                var interactiveFragment = TryBuildInteractiveAttributesFragment(node, baseLocation, ordinal);
                if (interactiveFragment is not null)
                {
                    ordinal++;
                    yield return interactiveFragment;
                }

                var contrastFragment = TryBuildColorContrastFragment(node, baseLocation, ordinal);
                if (contrastFragment is not null)
                {
                    ordinal++;
                    yield return contrastFragment;
                }

                var textStyleFragment = TryBuildTextStyleFragment(node, baseLocation, ordinal);
                if (textStyleFragment is not null)
                {
                    ordinal++;
                    yield return textStyleFragment;
                }
            }
        }
    }

    private static ColorContrastFragment? TryBuildColorContrastFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var style = node.GetAttributeValue("style", string.Empty);
        if (string.IsNullOrWhiteSpace(style))
        {
            return null;
        }

        var foreground = ExtractStyleProperty(style, "color");
        var background = ExtractStyleProperty(style, "background-color") ?? ExtractStyleProperty(style, "background");
        if (foreground is null || background is null)
        {
            return null;
        }

        if (!ColorContrastCalculator.TryParseColor(foreground, out var fg)
            || !ColorContrastCalculator.TryParseColor(background, out var bg))
        {
            return null;
        }

        var directText = HtmlEntity.DeEntitize(GetOwnText(node)).Trim();
        if (string.IsNullOrWhiteSpace(directText))
        {
            return null;
        }

        var fontSize = ParsePixelValue(ExtractStyleProperty(style, "font-size"));
        var fontWeight = ExtractStyleProperty(style, "font-weight");
        var isBold = fontWeight is "bold" or "bolder" || (fontWeight is not null && int.TryParse(fontWeight, out var weight) && weight >= 700);
        var isLargeText = fontSize is >= 24 || (fontSize is >= 18.66 && isBold);

        var ratio = ColorContrastCalculator.ComputeContrastRatio(fg, bg);
        var sampleText = directText.Length > 60 ? directText[..60] : directText;

        return new ColorContrastFragment(
            baseLocation with { Ordinal = ordinal },
            foreground,
            background,
            ratio,
            isLargeText,
            sampleText);
    }

    private static TextStyleFragment? TryBuildTextStyleFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var style = node.GetAttributeValue("style", string.Empty);
        if (string.IsNullOrWhiteSpace(style))
        {
            return null;
        }

        var textAlign = ExtractStyleProperty(style, "text-align");
        var fontSize = ParsePixelValue(ExtractStyleProperty(style, "font-size"));

        var directText = HtmlEntity.DeEntitize(GetOwnText(node)).Trim();
        if (string.IsNullOrWhiteSpace(directText))
        {
            return null;
        }

        var isJustified = string.Equals(textAlign, "justify", StringComparison.OrdinalIgnoreCase);
        var isTiny = fontSize is < 12;

        if (!isJustified && !isTiny)
        {
            return null;
        }

        var sampleText = directText.Length > 60 ? directText[..60] : directText;

        return new TextStyleFragment(
            baseLocation with { Ordinal = ordinal },
            textAlign,
            fontSize,
            sampleText);
    }

    private static string GetOwnText(HtmlNode node) =>
        string.Concat(node.ChildNodes.Where(c => c.NodeType == HtmlNodeType.Text).Select(c => c.InnerText));

    private static string? ExtractStyleProperty(string style, string propertyName)
    {
        foreach (var declaration in style.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = declaration.Split(':', 2);
            if (parts.Length != 2)
            {
                continue;
            }

            if (string.Equals(parts[0].Trim(), propertyName, StringComparison.OrdinalIgnoreCase))
            {
                return parts[1].Trim();
            }
        }

        return null;
    }

    private static double? ParsePixelValue(string? value)
    {
        if (value is null)
        {
            return null;
        }

        value = value.Trim();
        if (value.EndsWith("px", StringComparison.OrdinalIgnoreCase)
            && double.TryParse(value[..^2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var px))
        {
            return px;
        }

        return null;
    }

    private static ListStructureFragment BuildListStructureFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var itemCount = node.Elements("li").Count();
        var sampleText = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
        if (sampleText.Length > 80)
        {
            sampleText = sampleText[..80];
        }

        return new ListStructureFragment(
            baseLocation with { Ordinal = ordinal },
            "list",
            itemCount,
            sampleText);
    }

    private static ListStructureFragment? TryBuildFakeListFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        // Only consider leaf-ish blocks that aren't inside a real list already.
        if (node.Ancestors("ul").Any() || node.Ancestors("ol").Any())
        {
            return null;
        }

        var text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty);
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var matches = BulletLineRegex.Matches(text);
        if (matches.Count < 2)
        {
            return null;
        }

        var sampleText = text.Trim();
        if (sampleText.Length > 80)
        {
            sampleText = sampleText[..80];
        }

        return new ListStructureFragment(
            baseLocation with { Ordinal = ordinal },
            "fake-list",
            matches.Count,
            sampleText);
    }

    private static LanguageAttributeFragment? TryBuildLanguageAttributeFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        if (!node.Attributes.Contains("lang"))
        {
            return null;
        }

        var lang = node.GetAttributeValue("lang", string.Empty);
        var isWellFormed = System.Text.RegularExpressions.Regex.IsMatch(
            lang, @"^[a-zA-Z]{2,3}(-[a-zA-Z0-9]{2,8})*$");

        return new LanguageAttributeFragment(
            baseLocation with { Ordinal = ordinal },
            lang,
            isWellFormed);
    }

    private static InteractiveAttributesFragment? TryBuildInteractiveAttributesFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var hasTabIndex = node.Attributes.Contains("tabindex");
        var ariaHidden = string.Equals(node.GetAttributeValue("aria-hidden", string.Empty), "true", StringComparison.OrdinalIgnoreCase);
        var accessKey = node.Attributes.Contains("accesskey") ? node.GetAttributeValue("accesskey", string.Empty) : null;
        var role = node.Attributes.Contains("role") ? node.GetAttributeValue("role", string.Empty) : null;

        var isNativelyInteractive = NativelyInteractiveTags.Contains(node.Name)
            || (node.Name == "a" && node.Attributes.Contains("href"));

        // Only emit this fragment when there's something noteworthy: hidden-but-interactive
        // content, a positive tabindex, or an access key (all deterministic structural facts).
        int? tabIndex = null;
        if (hasTabIndex && int.TryParse(node.GetAttributeValue("tabindex", string.Empty), out var parsedTabIndex))
        {
            tabIndex = parsedTabIndex;
        }

        var isNoteworthy = (ariaHidden && isNativelyInteractive)
            || (tabIndex is > 0)
            || !string.IsNullOrEmpty(accessKey);

        if (!isNoteworthy)
        {
            return null;
        }

        return new InteractiveAttributesFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            tabIndex,
            ariaHidden,
            role,
            accessKey,
            isNativelyInteractive);
    }

    private static bool IsButtonLikeInput(HtmlNode node)
    {
        if (node.Name != "input")
        {
            return false;
        }

        var type = node.GetAttributeValue("type", string.Empty);
        return string.Equals(type, "submit", StringComparison.OrdinalIgnoreCase)
            || string.Equals(type, "button", StringComparison.OrdinalIgnoreCase)
            || string.Equals(type, "reset", StringComparison.OrdinalIgnoreCase)
            || string.Equals(type, "image", StringComparison.OrdinalIgnoreCase);
    }

    private static ImageFragment BuildImageFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var src = node.GetAttributeValue("src", string.Empty);
        var hasAlt = node.Attributes.Contains("alt");
        var alt = hasAlt ? node.GetAttributeValue("alt", string.Empty) : null;
        var isInsideLinkWithText = node.Ancestors("a").Any(a => !string.IsNullOrWhiteSpace(a.InnerText));

        return new ImageFragment(
            baseLocation with { Ordinal = ordinal },
            src,
            alt,
            isInsideLinkWithText);
    }

    private static LinkFragment BuildLinkFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var href = node.GetAttributeValue("href", string.Empty);
        var text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
        var isDocumentLink = DocumentExtensions.Any(ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

        return new LinkFragment(
            baseLocation with { Ordinal = ordinal },
            href,
            text,
            isDocumentLink);
    }

    private static HeadingFragment BuildHeadingFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var level = int.Parse(node.Name[1..]);
        var text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();

        return new HeadingFragment(
            baseLocation with { Ordinal = ordinal },
            level,
            text);
    }

    private static TableFragment BuildTableFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var hasCaption = node.SelectSingleNode(".//caption") is not null
            || node.Attributes.Contains("aria-label")
            || node.Attributes.Contains("aria-labelledby")
            || node.Attributes.Contains("summary");

        var firstRow = node.SelectSingleNode(".//tr");
        var hasHeaderRow = firstRow is not null
            && firstRow.Elements("th").Any();

        var hasHeaderCells = hasHeaderRow || node.SelectNodes(".//th") is not null;

        var columnCount = firstRow?.Elements("td").Concat(firstRow.Elements("th")).Count() ?? 0;

        return new TableFragment(
            baseLocation with { Ordinal = ordinal },
            hasHeaderCells,
            hasCaption,
            columnCount);
    }

    private static MediaFragment BuildMediaFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var src = node.GetAttributeValue("src", string.Empty);

        var hasTrack = node.SelectNodes(".//track") is { Count: > 0 } tracks
            && tracks.Any(t => string.Equals(t.GetAttributeValue("kind", string.Empty), "captions", StringComparison.OrdinalIgnoreCase)
                || string.Equals(t.GetAttributeValue("kind", string.Empty), "subtitles", StringComparison.OrdinalIgnoreCase));

        // A nearby sibling link mentioning "transcript" is a heuristic signal, not proof - the
        // rule layer treats this as NeedsReview rather than a deterministic pass/fail.
        var hasNearbyTranscriptLink = node.ParentNode?.SelectNodes(".//a") is { } links
            && links.Any(a => (a.InnerText ?? string.Empty).Contains("transcript", StringComparison.OrdinalIgnoreCase));

        return new MediaFragment(
            baseLocation with { Ordinal = ordinal },
            src,
            node.Name,
            hasTrack || hasNearbyTranscriptLink);
    }

    private static FormFieldFragment BuildFormFieldFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var inputType = node.Name == "input" ? node.GetAttributeValue("type", "text") : null;

        return new FormFieldFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            inputType,
            HasAccessibleName(node));
    }

    private static ButtonFragment BuildButtonFragment(HtmlNode node, SourceLocation baseLocation, int ordinal, string controlType)
    {
        var hasVisibleText = node.Name == "button"
            && !string.IsNullOrWhiteSpace(HtmlEntity.DeEntitize(node.InnerText ?? string.Empty));

        var hasValue = node.Name == "input" && !string.IsNullOrWhiteSpace(node.GetAttributeValue("value", string.Empty));

        var hasAccessibleName = hasVisibleText || hasValue || HasAriaLabel(node);

        return new ButtonFragment(
            baseLocation with { Ordinal = ordinal },
            controlType,
            hasAccessibleName);
    }

    private static IframeFragment BuildIframeFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var src = node.GetAttributeValue("src", string.Empty);
        var hasAccessibleName = node.Attributes.Contains("title") || HasAriaLabel(node);

        return new IframeFragment(
            baseLocation with { Ordinal = ordinal },
            src,
            hasAccessibleName);
    }

    private static bool HasAccessibleName(HtmlNode node)
    {
        if (HasAriaLabel(node))
        {
            return true;
        }

        var id = node.GetAttributeValue("id", string.Empty);
        if (!string.IsNullOrEmpty(id))
        {
            var owningDocument = node.OwnerDocument;
            var label = owningDocument.DocumentNode.SelectSingleNode($".//label[@for='{id}']");
            if (label is not null)
            {
                return true;
            }
        }

        // A control nested inside a <label> is implicitly labelled by that label's text.
        var ancestorLabel = node.Ancestors("label").FirstOrDefault();
        return ancestorLabel is not null && !string.IsNullOrWhiteSpace(HtmlEntity.DeEntitize(ancestorLabel.InnerText ?? string.Empty));
    }

    private static bool HasAriaLabel(HtmlNode node) =>
        !string.IsNullOrWhiteSpace(node.GetAttributeValue("aria-label", string.Empty))
        || !string.IsNullOrWhiteSpace(node.GetAttributeValue("aria-labelledby", string.Empty));
}
