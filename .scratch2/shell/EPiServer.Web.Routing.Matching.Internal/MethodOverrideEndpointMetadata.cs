using Microsoft.AspNetCore.Routing;

namespace EPiServer.Web.Routing.Matching.Internal;

internal class MethodOverrideEndpointMetadata : IDynamicEndpointMetadata
{
	public bool IsDynamic => true;
}
