using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Parsing;
using Xunit;

namespace OptiA11y.Core.Tests.Parsing;

public sealed class HtmlFragmentParserExtendedTests
{
    private static readonly SourceLocation BaseLocation = SourceLocation.OnProperty("content-1", "MainBody");

    [Fact]
    public void Paragraph_ProducesTextFragment()
    {
        const string html = "<p>This is a short paragraph.</p>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var text = Assert.Single(fragments.OfType<TextFragment>());

        Assert.Equal("This is a short paragraph.", text.Text);
        Assert.Null(text.LanguageCode);
    }

    [Fact]
    public void Paragraph_InheritsNearestAncestorLang()
    {
        const string html = "<div lang=\"fr\"><p>Bonjour le monde.</p></div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var text = Assert.Single(fragments.OfType<TextFragment>());

        Assert.Equal("fr", text.LanguageCode);
    }

    [Fact]
    public void ContainerWrappingParagraphs_DoesNotProduceDuplicateTextFragment()
    {
        const string html = "<div><p>First.</p><p>Second.</p></div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var textFragments = fragments.OfType<TextFragment>().ToList();

        Assert.Equal(2, textFragments.Count);
        Assert.Contains(textFragments, t => t.Text == "First.");
        Assert.Contains(textFragments, t => t.Text == "Second.");
    }

    [Fact]
    public void Link_WithVisibleText_HasAccessibleNameTrue()
    {
        const string html = "<a href=\"/about\">About us</a>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var link = Assert.Single(fragments.OfType<LinkFragment>());

        Assert.True(link.HasAccessibleName);
    }

    [Fact]
    public void Link_WithNoTextButAriaLabel_HasAccessibleNameTrue()
    {
        const string html = "<a href=\"/about\" aria-label=\"About us\"></a>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var link = Assert.Single(fragments.OfType<LinkFragment>());

        Assert.True(link.HasAccessibleName);
    }

    [Fact]
    public void Link_WithNoTextButImageWithAlt_HasAccessibleNameTrue()
    {
        const string html = "<a href=\"/about\"><img src=\"icon.png\" alt=\"About us\" /></a>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var link = Assert.Single(fragments.OfType<LinkFragment>());

        Assert.True(link.HasAccessibleName);
    }

    [Fact]
    public void Link_WithNoNamingMechanismAtAll_HasAccessibleNameFalse()
    {
        const string html = "<a href=\"/about\"></a>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var link = Assert.Single(fragments.OfType<LinkFragment>());

        Assert.False(link.HasAccessibleName);
    }

    [Fact]
    public void Link_WithTitleAttribute_CapturesTitle()
    {
        const string html = "<a href=\"/about\" title=\"About us\">About us</a>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var link = Assert.Single(fragments.OfType<LinkFragment>());

        Assert.Equal("About us", link.TitleAttribute);
    }

    [Fact]
    public void Svg_WithTitleChild_HasAccessibleNameTrue()
    {
        const string html = "<svg><title>Company logo</title></svg>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var svg = Assert.Single(fragments.OfType<NonTextElementFragment>());

        Assert.True(svg.HasAccessibleName);
    }

    [Fact]
    public void Svg_WithoutTitleOrAriaHidden_HasAccessibleNameFalse()
    {
        const string html = "<svg><path d=\"M0 0\" /></svg>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var svg = Assert.Single(fragments.OfType<NonTextElementFragment>());

        Assert.False(svg.HasAccessibleName);
        Assert.False(svg.IsAriaHidden);
    }

    [Fact]
    public void Svg_MarkedAriaHidden_IsAriaHiddenTrue()
    {
        const string html = "<svg aria-hidden=\"true\"><path d=\"M0 0\" /></svg>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var svg = Assert.Single(fragments.OfType<NonTextElementFragment>());

        Assert.True(svg.IsAriaHidden);
    }

    [Fact]
    public void Area_WithAlt_HasAccessibleNameTrue()
    {
        const string html = "<map><area shape=\"rect\" coords=\"0,0,10,10\" alt=\"Home\" href=\"/\" /></map>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var area = Assert.Single(fragments.OfType<NonTextElementFragment>());

        Assert.True(area.HasAccessibleName);
    }

    [Fact]
    public void Fieldset_WithLegend_HasLegendTrue()
    {
        const string html = "<fieldset><legend>Shipping</legend></fieldset>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var fieldset = Assert.Single(fragments.OfType<FieldsetFragment>());

        Assert.True(fieldset.HasLegend);
    }

    [Fact]
    public void Fieldset_WithoutLegend_HasLegendFalse()
    {
        const string html = "<fieldset><input type=\"text\" /></fieldset>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var fieldset = Assert.Single(fragments.OfType<FieldsetFragment>());

        Assert.False(fieldset.HasLegend);
    }

    [Fact]
    public void RadioGroup_WithoutFieldset_ProducesRadioGroupFragment()
    {
        const string html =
            "<input type=\"radio\" name=\"shipping\" value=\"standard\" />" +
            "<input type=\"radio\" name=\"shipping\" value=\"express\" />";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var radioGroup = Assert.Single(fragments.OfType<RadioGroupFragment>());

        Assert.Equal("shipping", radioGroup.GroupName);
        Assert.Equal(2, radioGroup.OptionCount);
    }

    [Fact]
    public void RadioGroup_WrappedInFieldset_ProducesNoRadioGroupFragment()
    {
        const string html =
            "<fieldset><legend>Shipping</legend>" +
            "<input type=\"radio\" name=\"shipping\" value=\"standard\" />" +
            "<input type=\"radio\" name=\"shipping\" value=\"express\" />" +
            "</fieldset>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();

        Assert.Empty(fragments.OfType<RadioGroupFragment>());
        Assert.Single(fragments.OfType<FieldsetFragment>());
    }

    [Fact]
    public void AriaLabelledBy_ResolvingWithinFragment_IsResolvedTrue()
    {
        const string html = "<h2 id=\"heading-1\">Section</h2><div aria-labelledby=\"heading-1\">Content</div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var reference = Assert.Single(fragments.OfType<AriaReferenceFragment>());

        Assert.True(reference.ResolvedWithinSameFragment);
    }

    [Fact]
    public void AriaLabelledBy_NotResolvingWithinFragment_IsResolvedFalse()
    {
        const string html = "<div aria-labelledby=\"missing-heading\">Content</div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var reference = Assert.Single(fragments.OfType<AriaReferenceFragment>());

        Assert.False(reference.ResolvedWithinSameFragment);
    }

    [Fact]
    public void InvalidRole_ProducesAriaAttributesFragment()
    {
        const string html = "<div role=\"banenr\">Content</div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var aria = Assert.Single(fragments.OfType<AriaAttributesFragment>());

        Assert.True(aria.RoleIsUnknown);
    }

    [Fact]
    public void MisspelledAriaAttribute_ProducesAriaAttributesFragment()
    {
        const string html = "<button aria-lable=\"Close\">X</button>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var aria = Assert.Single(fragments.OfType<AriaAttributesFragment>());

        Assert.Contains("aria-lable", aria.UnknownAriaAttributeNames);
    }

    [Fact]
    public void ValidRoleAndAriaAttributes_ProduceNoAriaAttributesFragment()
    {
        const string html = "<div role=\"navigation\" aria-hidden=\"false\">Content</div>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();

        Assert.Empty(fragments.OfType<AriaAttributesFragment>());
    }

    [Fact]
    public void ButtonNestedInsideLink_IsNestedInInteractiveAncestor()
    {
        const string html = "<a href=\"/x\"><button>Do it</button></a>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var nestedButton = Assert.Single(fragments.OfType<InteractiveAttributesFragment>(), f => f.TagName == "button");

        Assert.True(nestedButton.IsNestedInInteractiveAncestor);
    }

    [Fact]
    public void StandaloneButton_IsNotNestedInInteractiveAncestor()
    {
        const string html = "<button>Do it</button>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();

        Assert.Empty(fragments.OfType<InteractiveAttributesFragment>());
    }

    [Fact]
    public void FullyBoldShortParagraph_ProducesEmphasisBlockFragment()
    {
        const string html = "<p><strong>Getting started</strong></p>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var block = Assert.Single(fragments.OfType<EmphasisBlockFragment>());

        Assert.Equal("Getting started", block.SampleText);
    }

    [Fact]
    public void OrdinaryParagraph_ProducesNoEmphasisBlockFragment()
    {
        const string html = "<p>Just a normal paragraph with some <strong>emphasis in the middle</strong> of it.</p>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();

        Assert.Empty(fragments.OfType<EmphasisBlockFragment>());
    }

    [Fact]
    public void AutoplayingUnmutedVideo_CapturesAutoplayAndMuted()
    {
        const string html = "<video src=\"a.mp4\" autoplay></video>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var media = Assert.Single(fragments.OfType<MediaFragment>());

        Assert.True(media.Autoplay);
        Assert.False(media.Muted);
        Assert.False(media.HasControls);
    }

    [Fact]
    public void VideoWithDescriptionsTrack_HasDescriptionTrackTrue()
    {
        const string html = "<video src=\"a.mp4\"><track kind=\"descriptions\" src=\"desc.vtt\" /></video>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var media = Assert.Single(fragments.OfType<MediaFragment>());

        Assert.True(media.HasDescriptionTrack);
    }

    [Fact]
    public void TableWithMergedCellsMissingScope_FlagsMissingScope()
    {
        const string html =
            "<table><tr><th colspan=\"2\">Header</th></tr><tr><td>A</td><td>B</td></tr></table>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var table = Assert.Single(fragments.OfType<TableFragment>());

        Assert.True(table.HasMergedCells);
        Assert.False(table.AllHeaderCellsHaveScope);
    }

    [Fact]
    public void TableWithMergedCellsAndScope_AllHeaderCellsHaveScopeTrue()
    {
        const string html =
            "<table><tr><th colspan=\"2\" scope=\"col\">Header</th></tr><tr><td>A</td><td>B</td></tr></table>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var table = Assert.Single(fragments.OfType<TableFragment>());

        Assert.True(table.AllHeaderCellsHaveScope);
    }

    [Fact]
    public void TableWithInconsistentRowLengths_RowLengthsConsistentFalse()
    {
        const string html =
            "<table><tr><td>A</td><td>B</td></tr><tr><td>C</td></tr></table>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var table = Assert.Single(fragments.OfType<TableFragment>());

        Assert.False(table.HasMergedCells);
        Assert.False(table.RowLengthsConsistent);
    }

    [Fact]
    public void InputWithEmailyNameAndNoAutocomplete_InfersEmailPurpose()
    {
        const string html = "<input type=\"text\" name=\"customer-email\" />";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var field = Assert.Single(fragments.OfType<FormFieldFragment>());

        Assert.Equal("email", field.InferredPurposeCategory);
        Assert.Null(field.AutocompleteToken);
    }

    [Fact]
    public void InputWithAutocomplete_CapturesToken()
    {
        const string html = "<input type=\"email\" name=\"email\" autocomplete=\"email\" />";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var field = Assert.Single(fragments.OfType<FormFieldFragment>());

        Assert.Equal("email", field.AutocompleteToken);
    }
}
