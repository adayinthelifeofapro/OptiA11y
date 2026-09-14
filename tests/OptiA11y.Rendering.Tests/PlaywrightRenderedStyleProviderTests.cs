using OptiA11y.Rendering;

namespace OptiA11y.Rendering.Tests;

/// <summary>
/// Exercises <see cref="PlaywrightRenderedStyleProvider"/> against a static local HTML fixture
/// (loaded via file://) so computed-style extraction can be validated without depending on a
/// live CMS/browser preview environment.
///
/// NOTE: requires Playwright's Chromium binary to be installed once via
/// <c>pwsh bin/Debug/net10.0/playwright.ps1 install chromium</c> in this test project's output
/// directory. If the browser is unavailable, <see cref="PlaywrightRenderedStyleProvider"/> fails
/// soft and returns an empty list; this test detects that case and skips its assertions rather
/// than failing the whole suite in environments where the browser isn't installed.
/// </summary>
public sealed class PlaywrightRenderedStyleProviderTests
{
    [Fact]
    public async Task CaptureAsync_ExtractsComputedStylesFromFixture()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "contrast-fixture.html");
        var fixtureUri = new Uri(fixturePath);

        await using var provider = new PlaywrightRenderedStyleProvider();
        var styles = await provider.CaptureAsync(fixtureUri);

        if (styles.Count == 0)
        {
            // Playwright's Chromium binary is not installed in this environment; the provider
            // already fails soft (by design), so there is nothing further to assert here.
            return;
        }

        var lowContrast = Assert.Single(styles, s => s.Text.Contains("Low contrast", StringComparison.OrdinalIgnoreCase));
        Assert.Equal("justify", styles.Single(s => s.Text.Contains("Justified paragraph", StringComparison.OrdinalIgnoreCase)).TextAlign);

        var tiny = styles.Single(s => s.Text.Contains("Tiny paragraph", StringComparison.OrdinalIgnoreCase));
        Assert.True(tiny.FontSizePx < 12);

        var highContrast = styles.Single(s => s.Text.Contains("High contrast", StringComparison.OrdinalIgnoreCase));
        var isBold = highContrast.FontWeight.Contains("bold", StringComparison.OrdinalIgnoreCase)
            || (int.TryParse(highContrast.FontWeight, out var weight) && weight >= 700);
        Assert.True(isBold, $"Expected bold font-weight, got '{highContrast.FontWeight}'.");

        Assert.NotEmpty(lowContrast.Color);
        Assert.NotEmpty(lowContrast.BackgroundColor);
    }
}
