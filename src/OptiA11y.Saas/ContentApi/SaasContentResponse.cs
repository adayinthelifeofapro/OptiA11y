using System.Text.Json.Serialization;

namespace OptiA11y.Saas.ContentApi;

/// <summary>
/// A thin, isolated representation of the shape returned by the Optimizely SaaS Content API for
/// a single content item. This shape is UNVERIFIED against a real SaaS tenant (see the plan's
/// "verify before writing code" spike list) — keep all SaaS-specific mapping confined to this
/// namespace so remapping it later, once the real response shape is confirmed, cannot leak into
/// the rules engine or the PaaS adapter.
/// </summary>
public sealed class SaasContentResponse
{
    [JsonPropertyName("contentLink")]
    public string ContentLink { get; set; } = string.Empty;

    /// <summary>The content item's display name, used by OptiA11y.Core.Rules.PageTitle.PageTitleRule.</summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("properties")]
    public List<SaasContentProperty> Properties { get; set; } = new();
}

public sealed class SaasContentProperty
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("propertyDataType")]
    public string PropertyDataType { get; set; } = string.Empty;

    /// <summary>Raw HTML, present when <see cref="PropertyDataType"/> indicates rich text.</summary>
    [JsonPropertyName("html")]
    public string? Html { get; set; }

    /// <summary>Nested content items, present when <see cref="PropertyDataType"/> indicates a content area.</summary>
    [JsonPropertyName("contentArea")]
    public List<SaasContentResponse>? ContentArea { get; set; }
}
