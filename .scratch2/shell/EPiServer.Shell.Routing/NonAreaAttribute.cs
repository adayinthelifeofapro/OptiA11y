using System;

namespace EPiServer.Shell.Routing;

/// <summary>
///       Apply to shell module controller when controller should not be registered with area route
///       </summary>
[AttributeUsage(AttributeTargets.Class)]
public class NonAreaAttribute : Attribute
{
}
