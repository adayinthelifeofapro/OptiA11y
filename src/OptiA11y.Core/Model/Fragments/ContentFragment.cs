namespace OptiA11y.Core.Model.Fragments;

/// <summary>
/// Base type for every normalised piece of content the rules engine can inspect.
/// Adapters (PaaS, SaaS, later DOM) are responsible for producing these from their
/// respective sources; the engine has no knowledge of where a fragment came from.
/// </summary>
public abstract record ContentFragment(SourceLocation Location);
