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

    // Elements whose visible text is worth analysing as a standalone unit of prose (reading
    // level, sensory characteristics, shouting). Excludes containers like <div>/<ul> so the same
    // text isn't captured twice at multiple nesting levels.
    private static readonly HashSet<string> BlockTextTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "p", "li", "blockquote", "dd", "dt", "figcaption", "td", "th"
    };

    // If a candidate block-text element has one of these among its descendants, it's a
    // container wrapping further block-level content rather than a text leaf - skip it so we
    // capture the inner leaf instead of a duplicate of everything inside it.
    private static readonly HashSet<string> ContainerDescendantTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "p", "li", "div", "ul", "ol", "dl", "table", "blockquote"
    };

    // The fixed ARIA role vocabulary (WAI-ARIA 1.2 abstract roles excluded, since authors should
    // never use those directly). A role outside this set is not a "maybe" - it is simply not a
    // role that exists, which is what makes flagging it a deterministic Fail rather than NeedsReview.
    private static readonly HashSet<string> KnownAriaRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "alert", "alertdialog", "application", "article", "banner", "button", "cell", "checkbox",
        "columnheader", "combobox", "complementary", "contentinfo", "definition", "dialog",
        "directory", "document", "feed", "figure", "form", "grid", "gridcell", "group", "heading",
        "img", "link", "list", "listbox", "listitem", "log", "main", "marquee", "math", "menu",
        "menubar", "menuitem", "menuitemcheckbox", "menuitemradio", "navigation", "none", "note",
        "option", "presentation", "progressbar", "radio", "radiogroup", "region", "row", "rowgroup",
        "rowheader", "scrollbar", "search", "searchbox", "separator", "slider", "spinbutton",
        "status", "switch", "tab", "table", "tablist", "tabpanel", "term", "textbox", "timer",
        "toolbar", "tooltip", "tree", "treegrid", "treeitem"
    };

    // The fixed set of aria-* states and properties from the WAI-ARIA specification. An
    // attribute starting with "aria-" that isn't in this list is, deterministically, a typo or
    // an invented attribute - assistive technology will simply ignore it.
    private static readonly HashSet<string> KnownAriaAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        "aria-activedescendant", "aria-atomic", "aria-autocomplete", "aria-braillelabel",
        "aria-brailleroledescription", "aria-busy", "aria-checked", "aria-colcount", "aria-colindex",
        "aria-colindextext", "aria-colspan", "aria-controls", "aria-current", "aria-describedby",
        "aria-description", "aria-details", "aria-disabled", "aria-dropeffect", "aria-errormessage",
        "aria-expanded", "aria-flowto", "aria-grabbed", "aria-haspopup", "aria-hidden", "aria-invalid",
        "aria-keyshortcuts", "aria-label", "aria-labelledby", "aria-level", "aria-live", "aria-modal",
        "aria-multiline", "aria-multiselectable", "aria-orientation", "aria-owns", "aria-placeholder",
        "aria-posinset", "aria-pressed", "aria-readonly", "aria-relevant", "aria-required",
        "aria-roledescription", "aria-rowcount", "aria-rowindex", "aria-rowindextext", "aria-rowspan",
        "aria-selected", "aria-setsize", "aria-sort", "aria-valuemax", "aria-valuemin", "aria-valuenow",
        "aria-valuetext"
    };

    // Roles with a required set of owned elements per the ARIA spec, and the description used in
    // findings when a required owned element is missing. Scoped to a small, well-established
    // subset rather than the full specification. Matching is direct-or-descendant to tolerate
    // wrapper elements (e.g. a <ul> between a role="tablist" and its role="tab" children).
    private static readonly (string Role, string RequiredOwnedRole, string Description)[] RoleRequiredOwnedElements =
    {
        ("list", "listitem", "listitem"),
        ("listbox", "option", "option"),
        ("menu", "menuitem", "menuitem"),
        ("menubar", "menuitem", "menuitem"),
        ("radiogroup", "radio", "radio"),
        ("tablist", "tab", "tab"),
        ("table", "row", "row"),
        ("grid", "row", "row"),
        ("rowgroup", "row", "row"),
        ("row", "cell", "cell/gridcell/columnheader/rowheader")
    };

    // Roles with required state/property attributes per the ARIA spec. Scoped to a small,
    // well-established subset.
    private static readonly (string Role, string[] RequiredAttributes)[] RoleRequiredAttributesTable =
    {
        ("checkbox", new[] { "aria-checked" }),
        ("combobox", new[] { "aria-expanded" }),
        ("scrollbar", new[] { "aria-valuenow" }),
        ("slider", new[] { "aria-valuenow" }),
        ("spinbutton", new[] { "aria-valuenow" }),
        ("switch", new[] { "aria-checked" })
    };

    // Widget-state attributes and the roles that support them, per the ARIA spec. An element with
    // one of these attributes whose role isn't in the allowed set is misusing a valid ARIA
    // attribute - scoped to this small, well-established subset rather than the full
    // ARIA-in-HTML attribute-allowance tables.
    private static readonly (string Attribute, string[] AllowedRoles)[] WidgetStateAttributeAllowedRoles =
    {
        ("aria-checked", new[] { "checkbox", "radio", "switch", "menuitemcheckbox", "menuitemradio", "treeitem", "option" }),
        ("aria-selected", new[] { "option", "row", "tab", "gridcell", "rowheader", "columnheader", "treeitem" }),
        ("aria-expanded", new[] { "button", "combobox", "link", "tab", "treeitem", "row", "gridcell", "rowheader", "columnheader" }),
        ("aria-pressed", new[] { "button" })
    };

    // The element's implicit ARIA role by tag name, used to detect a role="..." attribute that
    // merely duplicates the element's native semantics (redundant-role) and to identify landmark
    // elements that don't carry an explicit role attribute.
    private static readonly Dictionary<string, string> ImplicitRoleByTag = new(StringComparer.OrdinalIgnoreCase)
    {
        ["button"] = "button",
        ["a"] = "link",
        ["nav"] = "navigation",
        ["main"] = "main",
        ["header"] = "banner",
        ["footer"] = "contentinfo",
        ["aside"] = "complementary",
        ["form"] = "form",
        ["table"] = "table",
        ["ul"] = "list",
        ["ol"] = "list",
        ["li"] = "listitem",
        ["img"] = "img",
        ["dialog"] = "dialog",
        ["article"] = "article",
        ["progress"] = "progressbar",
        ["select"] = "listbox",
        ["textarea"] = "textbox"
    };

    private static readonly HashSet<string> LandmarkRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "main", "banner", "navigation", "contentinfo", "complementary", "search", "form", "region"
    };

    private static readonly HashSet<string> ValidLivePolitenessValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "off", "polite", "assertive"
    };

    private static readonly HashSet<string> LiveRegionRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "status", "alert", "log", "marquee", "timer"
    };

    // Keyword -> WCAG 1.3.5 autocomplete-token category, used only to make an educated guess that
    // a field is a common identity field worth an autocomplete attribute. Deliberately a heuristic
    // signal: naming conventions vary too much for this to be a structural fact.
    private static readonly (string Category, string[] Keywords)[] InputPurposeKeywords =
    {
        ("email", new[] { "email", "e-mail" }),
        ("tel", new[] { "phone", "telephone", "mobile", "tel" }),
        ("name", new[] { "fullname", "full-name", "your-name", "yourname" }),
        ("given-name", new[] { "firstname", "first-name", "fname", "given-name" }),
        ("family-name", new[] { "lastname", "last-name", "lname", "surname", "family-name" }),
        ("postal-code", new[] { "zip", "zipcode", "postcode", "postal" }),
        ("street-address", new[] { "address", "street" }),
        ("cc-number", new[] { "cardnumber", "card-number", "creditcard" }),
        ("bday", new[] { "birthdate", "dob", "dateofbirth", "birthday" }),
        ("organization", new[] { "company", "organization", "organisation" })
    };


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

                case "figure":
                    yield return BuildFigureFragment(node, baseLocation, ordinal++);
                    break;

                case "a":
                    yield return BuildLinkFragment(node, baseLocation, ordinal++);
                    var skipLink = TryBuildSkipLinkFragment(node, baseLocation, ordinal);
                    if (skipLink is not null)
                    {
                        ordinal++;
                        yield return skipLink;
                    }
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

                case "dl":
                    var definitionList = TryBuildDefinitionListFragment(node, baseLocation, ordinal);
                    if (definitionList is not null)
                    {
                        ordinal++;
                        yield return definitionList;
                    }
                    break;

                case "p" or "div" or "span":
                    var fakeList = TryBuildFakeListFragment(node, baseLocation, ordinal);
                    if (fakeList is not null)
                    {
                        ordinal++;
                        yield return fakeList;
                    }

                    var emphasisBlock = TryBuildEmphasisBlockFragment(node, baseLocation, ordinal);
                    if (emphasisBlock is not null)
                    {
                        ordinal++;
                        yield return emphasisBlock;
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

                case "svg" or "area" or "object" or "embed":
                    yield return BuildNonTextElementFragment(node, baseLocation, ordinal++);
                    break;

                case "fieldset":
                    yield return BuildFieldsetFragment(node, baseLocation, ordinal++);
                    break;

                case "blockquote" or "sup" or "sub" or "b" or "i":
                    var markupSpan = TryBuildMarkupSpanFragment(node, baseLocation, ordinal);
                    if (markupSpan is not null)
                    {
                        ordinal++;
                        yield return markupSpan;
                    }
                    break;

                case "meta":
                    if (string.Equals(node.GetAttributeValue("http-equiv", string.Empty), "refresh", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return new MetaRefreshFragment(
                            baseLocation with { Ordinal = ordinal++ },
                            node.GetAttributeValue("content", string.Empty));
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

                var positionedContentFragment = TryBuildPositionedContentFragment(node, baseLocation, ordinal);
                if (positionedContentFragment is not null)
                {
                    ordinal++;
                    yield return positionedContentFragment;
                }

                var blinkingContentFragment = TryBuildBlinkingContentFragment(node, baseLocation, ordinal);
                if (blinkingContentFragment is not null)
                {
                    ordinal++;
                    yield return blinkingContentFragment;
                }

                var textFragment = TryBuildTextFragment(node, baseLocation, ordinal);
                if (textFragment is not null)
                {
                    ordinal++;
                    yield return textFragment;
                }

                var ariaAttributesFragment = TryBuildAriaAttributesFragment(node, baseLocation, ordinal);
                if (ariaAttributesFragment is not null)
                {
                    ordinal++;
                    yield return ariaAttributesFragment;
                }

                var ariaReferenceFragments = BuildAriaReferenceFragments(node, baseLocation, ordinal).ToList();
                foreach (var ariaReferenceFragment in ariaReferenceFragments)
                {
                    yield return ariaReferenceFragment;
                }
                ordinal += ariaReferenceFragments.Count;

                var ariaSemanticsFragment = TryBuildAriaSemanticsFragment(node, baseLocation, ordinal);
                if (ariaSemanticsFragment is not null)
                {
                    ordinal++;
                    yield return ariaSemanticsFragment;
                }

                var landmarkFragment = TryBuildLandmarkFragment(node, baseLocation, ordinal);
                if (landmarkFragment is not null)
                {
                    ordinal++;
                    yield return landmarkFragment;
                }

                var liveRegionFragment = TryBuildLiveRegionFragment(node, baseLocation, ordinal);
                if (liveRegionFragment is not null)
                {
                    ordinal++;
                    yield return liveRegionFragment;
                }

                var lineBreakRunFragment = TryBuildLineBreakRunFragment(node, baseLocation, ordinal);
                if (lineBreakRunFragment is not null)
                {
                    ordinal++;
                    yield return lineBreakRunFragment;
                }

                var nonTextContrastFragment = TryBuildNonTextContrastFragment(node, baseLocation, ordinal);
                if (nonTextContrastFragment is not null)
                {
                    ordinal++;
                    yield return nonTextContrastFragment;
                }
            }
        }

        var radioGroupFragments = DetectOrphanRadioGroups(doc, baseLocation, ordinal).ToList();
        foreach (var radioGroup in radioGroupFragments)
        {
            yield return radioGroup;
        }
        ordinal += radioGroupFragments.Count;

        foreach (var duplicateId in DetectDuplicateIds(doc, baseLocation, ordinal))
        {
            yield return duplicateId;
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

    private static NonTextContrastFragment? TryBuildNonTextContrastFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var style = node.GetAttributeValue("style", string.Empty);
        if (string.IsNullOrWhiteSpace(style))
        {
            return null;
        }

        var borderColor = ExtractStyleProperty(style, "border-color");
        var fillColor = ExtractStyleProperty(style, "fill");
        var foreground = borderColor ?? fillColor;
        if (foreground is null || !ColorContrastCalculator.TryParseColor(foreground, out var fg))
        {
            return null;
        }

        // Only meaningful for elements that don't also carry their own text/background contrast
        // check - a border on a plain box or an icon's fill, not a bordered text block.
        var directText = HtmlEntity.DeEntitize(GetOwnText(node)).Trim();
        if (!string.IsNullOrWhiteSpace(directText))
        {
            return null;
        }

        // Default to white when no ancestor sets an explicit background - a documented
        // assumption, not a guarantee (see NonTextContrastFragment's doc comment).
        var background = "#ffffff";
        for (var current = node.ParentNode; current is not null; current = current.ParentNode)
        {
            var ancestorStyle = current.GetAttributeValue("style", string.Empty);
            if (string.IsNullOrWhiteSpace(ancestorStyle))
            {
                continue;
            }

            var ancestorBackground = ExtractStyleProperty(ancestorStyle, "background-color") ?? ExtractStyleProperty(ancestorStyle, "background");
            if (ancestorBackground is not null)
            {
                background = ancestorBackground;
                break;
            }
        }

        if (!ColorContrastCalculator.TryParseColor(background, out var bg))
        {
            return null;
        }

        var ratio = ColorContrastCalculator.ComputeContrastRatio(fg, bg);
        var description = $"<{node.Name}>{(node.Attributes.Contains("class") ? $".{node.GetAttributeValue("class", string.Empty)}" : string.Empty)}";

        return new NonTextContrastFragment(
            baseLocation with { Ordinal = ordinal },
            foreground,
            background,
            ratio,
            description);
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
        var width = ParsePixelValue(ExtractStyleProperty(style, "width"));
        var lineHeight = ParseUnitlessNumber(ExtractStyleProperty(style, "line-height"));
        var hasBackgroundImage = !string.IsNullOrWhiteSpace(ExtractStyleProperty(style, "background-image"));

        var directText = HtmlEntity.DeEntitize(GetOwnText(node)).Trim();
        if (string.IsNullOrWhiteSpace(directText))
        {
            return null;
        }

        var isJustified = string.Equals(textAlign, "justify", StringComparison.OrdinalIgnoreCase);
        var isTiny = fontSize is < 12;
        var isWide = width is > 900;
        var isTightLineHeight = lineHeight is < 1.5;

        if (!isJustified && !isTiny && !isWide && !isTightLineHeight && !hasBackgroundImage)
        {
            return null;
        }

        var sampleText = directText.Length > 60 ? directText[..60] : directText;

        return new TextStyleFragment(
            baseLocation with { Ordinal = ordinal },
            textAlign,
            fontSize,
            sampleText,
            width,
            lineHeight,
            hasBackgroundImage);
    }

    private static PositionedContentFragment? TryBuildPositionedContentFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var style = node.GetAttributeValue("style", string.Empty);
        if (string.IsNullOrWhiteSpace(style))
        {
            return null;
        }

        var position = ExtractStyleProperty(style, "position");
        var floatValue = ExtractStyleProperty(style, "float");

        var isPositioned = position is "absolute" or "fixed";
        var isFloated = floatValue is "left" or "right";

        if (!isPositioned && !isFloated)
        {
            return null;
        }

        var directText = HtmlEntity.DeEntitize(GetOwnText(node)).Trim();
        if (string.IsNullOrWhiteSpace(directText))
        {
            return null;
        }

        var sampleText = directText.Length > 60 ? directText[..60] : directText;

        return new PositionedContentFragment(
            baseLocation with { Ordinal = ordinal },
            position ?? "static",
            floatValue,
            sampleText);
    }

    private static string GetOwnText(HtmlNode node) =>
        string.Concat(node.ChildNodes.Where(c => c.NodeType == HtmlNodeType.Text).Select(c => c.InnerText));

    private static BlinkingContentFragment? TryBuildBlinkingContentFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var isDeprecatedBlinkTag = string.Equals(node.Name, "blink", StringComparison.OrdinalIgnoreCase)
            || string.Equals(node.Name, "marquee", StringComparison.OrdinalIgnoreCase);

        var style = node.GetAttributeValue("style", string.Empty);
        var textDecoration = ExtractStyleProperty(style, "text-decoration");
        var animationName = ExtractStyleProperty(style, "animation") ?? ExtractStyleProperty(style, "animation-name");

        var hasBlinkStyle = (textDecoration?.Contains("blink", StringComparison.OrdinalIgnoreCase) ?? false)
            || (animationName?.Contains("blink", StringComparison.OrdinalIgnoreCase) ?? false);

        if (!isDeprecatedBlinkTag && !hasBlinkStyle)
        {
            return null;
        }

        var directText = HtmlEntity.DeEntitize(GetOwnText(node)).Trim();
        var description = directText.Length > 0
            ? $"<{node.Name}> \"{(directText.Length > 40 ? directText[..40] : directText)}\""
            : $"<{node.Name}>";

        return new BlinkingContentFragment(baseLocation with { Ordinal = ordinal }, description);
    }

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

    private static double? ParseUnitlessNumber(string? value)
    {
        if (value is null)
        {
            return null;
        }

        value = value.Trim();
        return double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var number)
            ? number
            : null;
    }

    private static ListStructureFragment BuildListStructureFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var itemCount = node.Elements("li").Count();
        var sampleText = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
        if (sampleText.Length > 80)
        {
            sampleText = sampleText[..80];
        }

        var hasNonListItemChild = node.ChildNodes
            .Any(child => child.NodeType == HtmlNodeType.Element && !string.Equals(child.Name, "li", StringComparison.OrdinalIgnoreCase));

        return new ListStructureFragment(
            baseLocation with { Ordinal = ordinal },
            "list",
            itemCount,
            sampleText,
            hasNonListItemChild);
    }

    private static MarkupSpanFragment? TryBuildMarkupSpanFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var hasCitation = node.Name == "blockquote"
            && (node.Attributes.Contains("cite") || node.Descendants("cite").Any());

        return new MarkupSpanFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            text,
            hasCitation);
    }

    private static DefinitionListFragment? TryBuildDefinitionListFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var children = node.ChildNodes
            .Where(c => c.NodeType == HtmlNodeType.Element && (c.Name is "dt" or "dd"))
            .ToList();

        if (children.Count == 0)
        {
            return null;
        }

        var descriptionBeforeTerm = false;
        var seenTerm = false;
        var hasOrphanedTerm = false;

        for (var i = 0; i < children.Count; i++)
        {
            if (children[i].Name == "dt")
            {
                seenTerm = true;
                var next = i + 1 < children.Count ? children[i + 1] : null;
                if (next is null || next.Name != "dd")
                {
                    hasOrphanedTerm = true;
                }
            }
            else if (children[i].Name == "dd" && !seenTerm)
            {
                descriptionBeforeTerm = true;
            }
        }

        return new DefinitionListFragment(
            baseLocation with { Ordinal = ordinal },
            hasOrphanedTerm,
            descriptionBeforeTerm);
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

        var isNestedInInteractiveAncestor = isNativelyInteractive
            && node.Ancestors().Any(a => NativelyInteractiveTags.Contains(a.Name)
                || (a.Name == "a" && a.Attributes.Contains("href")));

        var isNoteworthy = (ariaHidden && isNativelyInteractive)
            || (tabIndex is > 0)
            || !string.IsNullOrEmpty(accessKey)
            || isNestedInInteractiveAncestor;

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
            isNativelyInteractive,
            isNestedInInteractiveAncestor);
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

    private static FigureFragment BuildFigureFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var img = node.Descendants("img").FirstOrDefault();
        var altText = img is not null && img.Attributes.Contains("alt")
            ? img.GetAttributeValue("alt", string.Empty)
            : null;

        var captionNode = node.Descendants("figcaption").FirstOrDefault();
        var caption = captionNode is not null
            ? HtmlEntity.DeEntitize(captionNode.InnerText ?? string.Empty).Trim()
            : null;

        return new FigureFragment(
            baseLocation with { Ordinal = ordinal },
            altText,
            caption);
    }

    private static LinkFragment BuildLinkFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var href = node.GetAttributeValue("href", string.Empty);
        var text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
        var isDocumentLink = DocumentExtensions.Any(ext => href.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

        var hasImageWithAlt = node.Descendants("img")
            .Any(img => !string.IsNullOrWhiteSpace(img.GetAttributeValue("alt", string.Empty)));
        var hasAccessibleName = text.Length > 0 || HasAriaLabel(node) || hasImageWithAlt;

        var titleAttribute = node.Attributes.Contains("title") ? node.GetAttributeValue("title", string.Empty) : null;
        var target = node.Attributes.Contains("target") ? node.GetAttributeValue("target", string.Empty) : null;

        var style = node.GetAttributeValue("style", string.Empty);
        var hasExplicitColorStyle = !string.IsNullOrWhiteSpace(ExtractStyleProperty(style, "color"));
        var textDecoration = ExtractStyleProperty(style, "text-decoration");
        var removesUnderline = string.Equals(textDecoration, "none", StringComparison.OrdinalIgnoreCase);

        return new LinkFragment(
            baseLocation with { Ordinal = ordinal },
            href,
            text,
            isDocumentLink,
            hasAccessibleName,
            titleAttribute,
            target,
            GetNearestLang(node),
            hasExplicitColorStyle,
            removesUnderline);
    }

    private static SkipLinkFragment? TryBuildSkipLinkFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var href = node.GetAttributeValue("href", string.Empty);
        if (!href.StartsWith("#", StringComparison.Ordinal) || href.Length <= 1)
        {
            return null;
        }

        var targetId = href[1..];
        var resolved = node.OwnerDocument.DocumentNode.SelectSingleNode($".//*[@id='{targetId}']") is not null;

        return new SkipLinkFragment(baseLocation with { Ordinal = ordinal }, targetId, resolved);
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

        var cells = (IEnumerable<HtmlNode>?)node.SelectNodes(".//td|.//th") ?? Enumerable.Empty<HtmlNode>();
        var hasMergedCells = cells.Any(c => ParseSpan(c, "rowspan") > 1 || ParseSpan(c, "colspan") > 1);

        var headerCells = (IEnumerable<HtmlNode>?)node.SelectNodes(".//th") ?? Enumerable.Empty<HtmlNode>();
        var allHeaderCellsHaveScope = headerCells.All(th => !string.IsNullOrWhiteSpace(th.GetAttributeValue("scope", string.Empty)));

        var rows = node.SelectNodes(".//tr");
        var rowLengthsConsistent = rows is null
            || rows.Select(r => r.Elements("td").Concat(r.Elements("th")).Count()).Distinct().Count() <= 1;

        return new TableFragment(
            baseLocation with { Ordinal = ordinal },
            hasHeaderCells,
            hasCaption,
            columnCount,
            hasMergedCells,
            allHeaderCellsHaveScope,
            rowLengthsConsistent);
    }

    private static int ParseSpan(HtmlNode cell, string attributeName) =>
        int.TryParse(cell.GetAttributeValue(attributeName, "1"), out var span) ? span : 1;

    private static MediaFragment BuildMediaFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var src = node.GetAttributeValue("src", string.Empty);

        var tracks = node.SelectNodes(".//track");

        var hasTrack = tracks is { Count: > 0 }
            && tracks.Any(t => string.Equals(t.GetAttributeValue("kind", string.Empty), "captions", StringComparison.OrdinalIgnoreCase)
                || string.Equals(t.GetAttributeValue("kind", string.Empty), "subtitles", StringComparison.OrdinalIgnoreCase));

        var hasDescriptionTrack = tracks is { Count: > 0 }
            && tracks.Any(t => string.Equals(t.GetAttributeValue("kind", string.Empty), "descriptions", StringComparison.OrdinalIgnoreCase));

        // A nearby sibling link mentioning "transcript" is a heuristic signal, not proof - the
        // rule layer treats this as NeedsReview rather than a deterministic pass/fail.
        var transcriptLink = node.ParentNode?.SelectNodes(".//a")
            ?.FirstOrDefault(a => (a.InnerText ?? string.Empty).Contains("transcript", StringComparison.OrdinalIgnoreCase));
        var hasNearbyTranscriptLink = transcriptLink is not null;

        var hasSignLanguageTrack = tracks is { Count: > 0 }
            && tracks.Any(t => string.Equals(t.GetAttributeValue("kind", string.Empty), "sign", StringComparison.OrdinalIgnoreCase)
                || t.GetAttributeValue("label", string.Empty).Contains("sign language", StringComparison.OrdinalIgnoreCase));

        var hasExtendedDescriptionTrack = tracks is { Count: > 0 }
            && tracks.Any(t => string.Equals(t.GetAttributeValue("kind", string.Empty), "descriptions", StringComparison.OrdinalIgnoreCase)
                && t.GetAttributeValue("label", string.Empty).Contains("extended", StringComparison.OrdinalIgnoreCase));

        var flashIndicatorPattern = src.Contains("flash", StringComparison.OrdinalIgnoreCase)
            || src.Contains("strobe", StringComparison.OrdinalIgnoreCase)
            || src.Contains("flicker", StringComparison.OrdinalIgnoreCase)
            || node.GetAttributeValue("class", string.Empty).Contains("flash", StringComparison.OrdinalIgnoreCase)
            || node.GetAttributeValue("data-flash", string.Empty).Length > 0;

        var autoplay = node.Attributes.Contains("autoplay");
        var muted = node.Attributes.Contains("muted");
        var hasControls = node.Attributes.Contains("controls");

        return new MediaFragment(
            baseLocation with { Ordinal = ordinal },
            src,
            node.Name,
            hasTrack || hasNearbyTranscriptLink,
            autoplay,
            muted,
            hasControls,
            hasDescriptionTrack,
            hasSignLanguageTrack,
            hasExtendedDescriptionTrack,
            transcriptLink?.GetAttributeValue("href", string.Empty),
            transcriptLink is not null ? HtmlEntity.DeEntitize(transcriptLink.InnerText ?? string.Empty).Trim() : null,
            flashIndicatorPattern);
    }

    private static FormFieldFragment BuildFormFieldFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var inputType = node.Name == "input" ? node.GetAttributeValue("type", "text") : null;
        var autocompleteToken = node.Attributes.Contains("autocomplete")
            ? node.GetAttributeValue("autocomplete", string.Empty)
            : null;

        var placeholder = node.Attributes.Contains("placeholder")
            ? node.GetAttributeValue("placeholder", string.Empty)
            : null;

        var isRequired = node.Attributes.Contains("required")
            || string.Equals(node.GetAttributeValue("aria-required", string.Empty), "true", StringComparison.OrdinalIgnoreCase);

        var labelText = GetAssociatedLabelText(node);
        var hasVisualRequiredIndicator = (labelText is not null && labelText.Contains('*'))
            || (placeholder is not null && placeholder.Contains('*'));

        var hasAriaDescribedBy = !string.IsNullOrWhiteSpace(node.GetAttributeValue("aria-describedby", string.Empty));

        var isDisabled = node.Attributes.Contains("disabled");
        var ariaDisabled = string.Equals(node.GetAttributeValue("aria-disabled", string.Empty), "true", StringComparison.OrdinalIgnoreCase);
        var ariaInvalid = string.Equals(node.GetAttributeValue("aria-invalid", string.Empty), "true", StringComparison.OrdinalIgnoreCase);

        var formatConstrainedInputTypes = new[] { "email", "tel", "url", "date", "time", "datetime-local", "month", "week", "number" };
        var hasPatternOrFormatConstraint = node.Attributes.Contains("pattern")
            || (inputType is not null && formatConstrainedInputTypes.Contains(inputType, StringComparer.OrdinalIgnoreCase));

        string? firstOptionText = null;
        var hasEmptyOptionText = false;
        if (node.Name == "select")
        {
            var options = node.SelectNodes(".//option");
            if (options is not null && options.Count > 0)
            {
                firstOptionText = HtmlEntity.DeEntitize(options[0].InnerText ?? string.Empty).Trim();
                hasEmptyOptionText = options.Any(o => string.IsNullOrWhiteSpace(HtmlEntity.DeEntitize(o.InnerText ?? string.Empty)));
            }
        }

        return new FormFieldFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            inputType,
            HasAccessibleName(node),
            autocompleteToken,
            InferPurposeCategory(node),
            labelText,
            placeholder,
            isRequired,
            hasVisualRequiredIndicator,
            hasAriaDescribedBy,
            isDisabled,
            ariaDisabled,
            firstOptionText,
            hasEmptyOptionText,
            ariaInvalid,
            hasPatternOrFormatConstraint);
    }

    private static string? GetAssociatedLabelText(HtmlNode node)
    {
        var id = node.GetAttributeValue("id", string.Empty);
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        var label = node.OwnerDocument.DocumentNode.SelectSingleNode($".//label[@for='{id}']");
        var text = label is not null ? HtmlEntity.DeEntitize(label.InnerText ?? string.Empty).Trim() : null;
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    private static string? InferPurposeCategory(HtmlNode node)
    {
        var id = node.GetAttributeValue("id", string.Empty);
        var name = node.GetAttributeValue("name", string.Empty);
        var placeholder = node.GetAttributeValue("placeholder", string.Empty);

        var labelText = string.Empty;
        if (!string.IsNullOrEmpty(id))
        {
            labelText = node.OwnerDocument.DocumentNode.SelectSingleNode($".//label[@for='{id}']")?.InnerText ?? string.Empty;
        }

        var haystack = HtmlEntity.DeEntitize($"{id} {name} {placeholder} {labelText}").ToLowerInvariant();

        foreach (var (category, keywords) in InputPurposeKeywords)
        {
            if (keywords.Any(keyword => haystack.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                return category;
            }
        }

        return null;
    }

    private static ButtonFragment BuildButtonFragment(HtmlNode node, SourceLocation baseLocation, int ordinal, string controlType)
    {
        var hasVisibleText = node.Name == "button"
            && !string.IsNullOrWhiteSpace(HtmlEntity.DeEntitize(node.InnerText ?? string.Empty));

        var hasValue = node.Name == "input" && !string.IsNullOrWhiteSpace(node.GetAttributeValue("value", string.Empty));

        var hasAccessibleName = hasVisibleText || hasValue || HasAriaLabel(node);
        var titleAttribute = node.Attributes.Contains("title") ? node.GetAttributeValue("title", string.Empty) : null;

        return new ButtonFragment(
            baseLocation with { Ordinal = ordinal },
            controlType,
            hasAccessibleName,
            titleAttribute);
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

    private static NonTextElementFragment BuildNonTextElementFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var isAriaHidden = string.Equals(node.GetAttributeValue("aria-hidden", string.Empty), "true", StringComparison.OrdinalIgnoreCase);

        var hasAccessibleName = node.Name switch
        {
            "svg" => node.Elements("title").Any(t => !string.IsNullOrWhiteSpace(HtmlEntity.DeEntitize(t.InnerText ?? string.Empty)))
                || HasAriaLabel(node),
            "area" => !string.IsNullOrWhiteSpace(node.GetAttributeValue("alt", string.Empty)) || HasAriaLabel(node),
            _ => node.Attributes.Contains("title") && !string.IsNullOrWhiteSpace(node.GetAttributeValue("title", string.Empty))
                || HasAriaLabel(node)
                || !string.IsNullOrWhiteSpace(HtmlEntity.DeEntitize(node.InnerText ?? string.Empty))
        };

        return new NonTextElementFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            hasAccessibleName,
            isAriaHidden);
    }

    private static FieldsetFragment BuildFieldsetFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var hasLegend = node.Elements("legend").Any();

        return new FieldsetFragment(
            baseLocation with { Ordinal = ordinal },
            hasLegend);
    }

    private static EmphasisBlockFragment? TryBuildEmphasisBlockFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        // Restrict to <p> to avoid over-firing on <div>/<span> wrapper elements, which are used
        // for all sorts of non-prose layout purposes.
        if (node.Name != "p")
        {
            return null;
        }

        var text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(text) || text.Length > 120)
        {
            return null;
        }

        var elementChildren = node.ChildNodes.Where(c => c.NodeType == HtmlNodeType.Element).ToList();
        var isFullyBold = elementChildren.Count > 0
            && elementChildren.All(c => c.Name is "b" or "strong")
            && node.ChildNodes.Where(c => c.NodeType == HtmlNodeType.Text)
                .All(c => string.IsNullOrWhiteSpace(c.InnerText));

        var style = node.GetAttributeValue("style", string.Empty);
        var fontSize = ParsePixelValue(ExtractStyleProperty(style, "font-size"));
        var isLargeFont = fontSize is >= 18;

        if (!isFullyBold && !isLargeFont)
        {
            return null;
        }

        return new EmphasisBlockFragment(
            baseLocation with { Ordinal = ordinal },
            text.Length > 80 ? text[..80] : text,
            text.Length);
    }

    private static TextFragment? TryBuildTextFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        if (!BlockTextTags.Contains(node.Name))
        {
            return null;
        }

        // Skip containers that wrap further block-level content - the inner leaf will be
        // captured on its own, rather than duplicating its text here too.
        if (node.Descendants().Any(d => ContainerDescendantTags.Contains(d.Name)))
        {
            return null;
        }

        var text = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return new TextFragment(
            baseLocation with { Ordinal = ordinal },
            text,
            GetNearestLang(node));
    }

    private static LineBreakRunFragment? TryBuildLineBreakRunFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        if (!string.Equals(node.Name, "br", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        // Only report from the first <br> in a run, counting how many consecutive <br> siblings
        // follow it (ignoring whitespace-only text nodes between them), so a run isn't reported once per element.
        var previousSibling = node.PreviousSibling;
        while (previousSibling is not null && previousSibling.NodeType == HtmlNodeType.Text && string.IsNullOrWhiteSpace(previousSibling.InnerText))
        {
            previousSibling = previousSibling.PreviousSibling;
        }

        if (previousSibling is not null && string.Equals(previousSibling.Name, "br", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var count = 1;
        var current = node.NextSibling;
        while (current is not null)
        {
            if (current.NodeType == HtmlNodeType.Text && string.IsNullOrWhiteSpace(current.InnerText))
            {
                current = current.NextSibling;
                continue;
            }

            if (!string.Equals(current.Name, "br", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            count++;
            current = current.NextSibling;
        }

        return count < 2
            ? null
            : new LineBreakRunFragment(baseLocation with { Ordinal = ordinal }, count);
    }

    private static string? GetNearestLang(HtmlNode node)
    {
        for (var current = node; current is not null; current = current.ParentNode)
        {
            if (current.Attributes.Contains("lang"))
            {
                var lang = current.GetAttributeValue("lang", string.Empty);
                if (!string.IsNullOrWhiteSpace(lang))
                {
                    return lang;
                }
            }
        }

        return null;
    }

    private static AriaAttributesFragment? TryBuildAriaAttributesFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var role = node.Attributes.Contains("role") ? node.GetAttributeValue("role", string.Empty) : null;

        var roleIsUnknown = !string.IsNullOrWhiteSpace(role)
            && role.Split(' ', StringSplitOptions.RemoveEmptyEntries).Any(r => !KnownAriaRoles.Contains(r));

        var unknownAriaAttributeNames = node.Attributes
            .Select(a => a.Name)
            .Where(name => name.StartsWith("aria-", StringComparison.OrdinalIgnoreCase) && !KnownAriaAttributes.Contains(name))
            .ToList();

        // Only emit when there's genuinely something invalid - matches the "noteworthy only"
        // pattern used elsewhere in the parser to keep fragment volume down.
        if (!roleIsUnknown && unknownAriaAttributeNames.Count == 0)
        {
            return null;
        }

        return new AriaAttributesFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            role,
            roleIsUnknown,
            unknownAriaAttributeNames);
    }

    private static bool IsNativelyFocusable(HtmlNode node) =>
        NativelyInteractiveTags.Contains(node.Name) || (node.Name == "a" && node.Attributes.Contains("href"));

    private static AriaSemanticsFragment? TryBuildAriaSemanticsFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var role = node.Attributes.Contains("role") ? node.GetAttributeValue("role", string.Empty) : null;
        var primaryRole = role?.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

        string? missingRequiredOwnedElementDescription = null;
        if (primaryRole is not null)
        {
            var match = RoleRequiredOwnedElements.FirstOrDefault(r => string.Equals(r.Role, primaryRole, StringComparison.OrdinalIgnoreCase));
            if (match.Role is not null
                && !node.Descendants().Any(d => string.Equals(d.GetAttributeValue("role", string.Empty), match.RequiredOwnedRole, StringComparison.OrdinalIgnoreCase)))
            {
                missingRequiredOwnedElementDescription = match.Description;
            }
        }

        var missingRequiredAttributes = new List<string>();
        if (primaryRole is not null)
        {
            var match = RoleRequiredAttributesTable.FirstOrDefault(r => string.Equals(r.Role, primaryRole, StringComparison.OrdinalIgnoreCase));
            if (match.Role is not null)
            {
                missingRequiredAttributes.AddRange(match.RequiredAttributes.Where(attr => !node.Attributes.Contains(attr)));
            }
        }

        var disallowedWidgetStateAttributes = new List<string>();
        foreach (var (attribute, allowedRoles) in WidgetStateAttributeAllowedRoles)
        {
            if (!node.Attributes.Contains(attribute))
            {
                continue;
            }

            var effectiveRole = primaryRole ?? (ImplicitRoleByTag.TryGetValue(node.Name, out var implicitRole) ? implicitRole : null);
            if (effectiveRole is null || !allowedRoles.Contains(effectiveRole, StringComparer.OrdinalIgnoreCase))
            {
                disallowedWidgetStateAttributes.Add(attribute);
            }
        }

        var ariaHidden = string.Equals(node.GetAttributeValue("aria-hidden", string.Empty), "true", StringComparison.OrdinalIgnoreCase);
        var isAriaHiddenWithFocusableDescendant = ariaHidden && node.Descendants().Any(IsNativelyFocusable);

        var isFocusable = IsNativelyFocusable(node) || (node.Attributes.Contains("tabindex")
            && int.TryParse(node.GetAttributeValue("tabindex", string.Empty), out var tabIndex) && tabIndex >= 0);

        var hasGlobalAriaAttribute = node.Attributes
            .Any(a => a.Name.StartsWith("aria-", StringComparison.OrdinalIgnoreCase) && KnownAriaAttributes.Contains(a.Name));

        var isRedundantRole = primaryRole is not null
            && ImplicitRoleByTag.TryGetValue(node.Name, out var nativeRole)
            && string.Equals(primaryRole, nativeRole, StringComparison.OrdinalIgnoreCase);

        var isNoteworthy = missingRequiredOwnedElementDescription is not null
            || missingRequiredAttributes.Count > 0
            || disallowedWidgetStateAttributes.Count > 0
            || isAriaHiddenWithFocusableDescendant
            || isRedundantRole
            || (string.Equals(primaryRole, "presentation", StringComparison.OrdinalIgnoreCase) || string.Equals(primaryRole, "none", StringComparison.OrdinalIgnoreCase))
               && (isFocusable || hasGlobalAriaAttribute);

        if (!isNoteworthy)
        {
            return null;
        }

        return new AriaSemanticsFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            role,
            missingRequiredOwnedElementDescription,
            missingRequiredAttributes,
            disallowedWidgetStateAttributes,
            isAriaHiddenWithFocusableDescendant,
            isFocusable,
            hasGlobalAriaAttribute,
            isRedundantRole);
    }

    private static LandmarkFragment? TryBuildLandmarkFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var explicitRole = node.Attributes.Contains("role") ? node.GetAttributeValue("role", string.Empty) : null;
        var isExplicitRole = !string.IsNullOrWhiteSpace(explicitRole) && LandmarkRoles.Contains(explicitRole);

        string? effectiveRole = isExplicitRole
            ? explicitRole
            : (ImplicitRoleByTag.TryGetValue(node.Name, out var implicitRole) && LandmarkRoles.Contains(implicitRole) ? implicitRole : null);

        if (effectiveRole is null)
        {
            return null;
        }

        var hasAccessibleName = node.Attributes.Contains("aria-label") || node.Attributes.Contains("aria-labelledby");

        return new LandmarkFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            effectiveRole,
            isExplicitRole,
            hasAccessibleName);
    }

    private static LiveRegionFragment? TryBuildLiveRegionFragment(HtmlNode node, SourceLocation baseLocation, int ordinal)
    {
        var ariaLiveValue = node.Attributes.Contains("aria-live") ? node.GetAttributeValue("aria-live", string.Empty) : null;
        var role = node.Attributes.Contains("role") ? node.GetAttributeValue("role", string.Empty) : null;
        var hasLiveRegionRole = role is not null && LiveRegionRoles.Contains(role);

        if (ariaLiveValue is null && !hasLiveRegionRole)
        {
            return null;
        }

        var hasInvalidPolitenessValue = ariaLiveValue is not null && !ValidLivePolitenessValues.Contains(ariaLiveValue);
        var textLength = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty).Trim().Length;

        return new LiveRegionFragment(
            baseLocation with { Ordinal = ordinal },
            node.Name,
            role,
            ariaLiveValue,
            hasInvalidPolitenessValue,
            textLength);
    }

    private static IEnumerable<AriaReferenceFragment> BuildAriaReferenceFragments(HtmlNode node, SourceLocation baseLocation, int startingOrdinal)
    {
        var ordinal = startingOrdinal;
        var owningDocument = node.OwnerDocument;

        foreach (var attributeName in new[] { "aria-labelledby", "aria-describedby" })
        {
            var value = node.GetAttributeValue(attributeName, string.Empty);
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            foreach (var id in value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var resolved = owningDocument.DocumentNode.SelectSingleNode($".//*[@id='{id}']") is not null;
                yield return new AriaReferenceFragment(baseLocation with { Ordinal = ordinal++ }, attributeName, id, resolved);
            }
        }

        if (node.Name == "label")
        {
            var forId = node.GetAttributeValue("for", string.Empty);
            if (!string.IsNullOrWhiteSpace(forId))
            {
                var resolved = owningDocument.DocumentNode.SelectSingleNode($".//*[@id='{forId}']") is not null;
                yield return new AriaReferenceFragment(baseLocation with { Ordinal = ordinal }, "for", forId, resolved);
            }
        }
    }

    private static IEnumerable<RadioGroupFragment> DetectOrphanRadioGroups(HtmlDocument doc, SourceLocation baseLocation, int startingOrdinal)
    {
        var ordinal = startingOrdinal;

        var radioGroups = doc.DocumentNode.Descendants("input")
            .Where(i => string.Equals(i.GetAttributeValue("type", string.Empty), "radio", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(i.GetAttributeValue("name", string.Empty)))
            .GroupBy(i => i.GetAttributeValue("name", string.Empty), StringComparer.OrdinalIgnoreCase);

        foreach (var group in radioGroups)
        {
            var inputs = group.ToList();
            if (inputs.Count < 2)
            {
                continue;
            }

            var hasCommonFieldset = inputs[0].Ancestors("fieldset")
                .Any(fieldset => inputs.All(i => i.Ancestors("fieldset").Contains(fieldset)));

            if (hasCommonFieldset)
            {
                // A fieldset without a legend is already reported by the plain FieldsetFragment
                // check emitted for that element - don't double-report the same underlying issue.
                continue;
            }

            yield return new RadioGroupFragment(
                baseLocation with { Ordinal = ordinal++ },
                group.Key,
                inputs.Count);
        }
    }

    private static IEnumerable<DuplicateIdFragment> DetectDuplicateIds(HtmlDocument doc, SourceLocation baseLocation, int startingOrdinal)
    {
        var ordinal = startingOrdinal;

        var duplicates = doc.DocumentNode.Descendants()
            .Where(n => n.NodeType == HtmlNodeType.Element && n.Attributes.Contains("id"))
            .Select(n => n.GetAttributeValue("id", string.Empty))
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .GroupBy(id => id, StringComparer.Ordinal)
            .Where(g => g.Count() > 1);

        foreach (var group in duplicates)
        {
            yield return new DuplicateIdFragment(
                baseLocation with { Ordinal = ordinal++ },
                group.Key,
                group.Count());
        }
    }
}
