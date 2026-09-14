using OptiA11y.Core.Model;

namespace OptiA11y.Core.Tests;

internal static class TestLocations
{
    public static SourceLocation OnMainBody(int ordinal = 0) =>
        SourceLocation.OnProperty("content-1", "MainBody", ordinal);
}
