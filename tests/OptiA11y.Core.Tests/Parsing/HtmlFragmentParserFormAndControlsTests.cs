using OptiA11y.Core.Model;
using OptiA11y.Core.Model.Fragments;
using OptiA11y.Core.Parsing;
using Xunit;

namespace OptiA11y.Core.Tests.Parsing;

public sealed class HtmlFragmentParserFormAndControlsTests
{
    private static readonly SourceLocation BaseLocation = SourceLocation.OnProperty("content-1", "MainBody");

    [Fact]
    public void Input_WithAssociatedLabelFor_HasAccessibleNameTrue()
    {
        const string html = "<label for=\"email\">Email</label><input id=\"email\" type=\"text\" />";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var field = Assert.Single(fragments.OfType<FormFieldFragment>());

        Assert.True(field.HasAccessibleName);
        Assert.Equal("text", field.InputType);
    }

    [Fact]
    public void Input_WrappedInLabel_HasAccessibleNameTrue()
    {
        const string html = "<label>Email <input type=\"text\" /></label>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var field = Assert.Single(fragments.OfType<FormFieldFragment>());

        Assert.True(field.HasAccessibleName);
    }

    [Fact]
    public void Input_WithoutLabel_HasAccessibleNameFalse()
    {
        const string html = "<input type=\"text\" />";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var field = Assert.Single(fragments.OfType<FormFieldFragment>());

        Assert.False(field.HasAccessibleName);
    }

    [Fact]
    public void SubmitInput_IsExtractedAsButton()
    {
        const string html = "<input type=\"submit\" value=\"Send\" />";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var button = Assert.Single(fragments.OfType<ButtonFragment>());

        Assert.True(button.HasAccessibleName);
        Assert.Empty(fragments.OfType<FormFieldFragment>());
    }

    [Fact]
    public void Button_WithText_HasAccessibleNameTrue()
    {
        const string html = "<button>Submit</button>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var button = Assert.Single(fragments.OfType<ButtonFragment>());

        Assert.True(button.HasAccessibleName);
    }

    [Fact]
    public void Button_WithoutTextOrAriaLabel_HasAccessibleNameFalse()
    {
        const string html = "<button></button>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var button = Assert.Single(fragments.OfType<ButtonFragment>());

        Assert.False(button.HasAccessibleName);
    }

    [Fact]
    public void Button_WithAriaLabel_HasAccessibleNameTrue()
    {
        const string html = "<button aria-label=\"Close dialog\"></button>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var button = Assert.Single(fragments.OfType<ButtonFragment>());

        Assert.True(button.HasAccessibleName);
    }

    [Fact]
    public void Iframe_WithTitle_HasAccessibleNameTrue()
    {
        const string html = "<iframe src=\"https://example.com\" title=\"Embedded map\"></iframe>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var iframe = Assert.Single(fragments.OfType<IframeFragment>());

        Assert.True(iframe.HasAccessibleName);
    }

    [Fact]
    public void Iframe_WithoutTitle_HasAccessibleNameFalse()
    {
        const string html = "<iframe src=\"https://example.com\"></iframe>";

        var fragments = HtmlFragmentParser.Parse(html, BaseLocation).ToList();
        var iframe = Assert.Single(fragments.OfType<IframeFragment>());

        Assert.False(iframe.HasAccessibleName);
    }
}
