using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Shell;
using EPiServer.Shell.Services.Rest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Web.Routing.Matching.Internal;

internal class MethodOverrideMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
{
	private EndpointDataSource _shellEndpoints;

	private readonly ConcurrentDictionary<string, Endpoint> _urlEndpoints = new ConcurrentDictionary<string, Endpoint>();

	private const string urlPrefix = "/stores/";

	public override int Order => 1;

	public void SetShellEndpoints(EndpointDataSource shellEndpoints)
	{
		_shellEndpoints = shellEndpoints;
	}

	public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
	{
		return endpoints.Any((Endpoint e) => e.Metadata.GetMetadata<MethodOverrideEndpointMetadata>() != null);
	}

	public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
	{
		for (int i = 0; i < candidates.Count; i++)
		{
			if (candidates[i].Endpoint?.Metadata.GetMetadata<MethodOverrideEndpointMetadata>() == null)
			{
				continue;
			}
			string method = httpContext.Request.Headers["X-Http-Method-Override"].FirstOrDefault();
			if (method != null)
			{
				string text = httpContext.Request.Path.ToString();
				if (!text.EndsWith('/'))
				{
					text += "/";
				}
				string key = UriHelper.BuildRelative(httpContext.Request.PathBase, text, httpContext.Request.QueryString);
				key += method;
				Endpoint orAdd = _urlEndpoints.GetOrAdd(key, (string k) => _shellEndpoints.Endpoints.FirstOrDefault(delegate(Endpoint e)
				{
					if (e.Metadata.GetMetadata<ActionDescriptor>() is ControllerActionDescriptor controllerActionDescriptor)
					{
						RestStoreAttribute restStoreAttribute = controllerActionDescriptor.EndpointMetadata.FirstOfType<RestStoreAttribute>();
						if (restStoreAttribute == null)
						{
							return false;
						}
						if (key.Contains("/stores/" + restStoreAttribute.StoreName + "/", StringComparison.OrdinalIgnoreCase) && controllerActionDescriptor.ActionName.Equals(method, StringComparison.OrdinalIgnoreCase))
						{
							return true;
						}
					}
					return false;
				}));
				if (orAdd != null)
				{
					ActionDescriptor metadata = orAdd.Metadata.GetMetadata<ActionDescriptor>();
					ActionContext controllerContext = new ActionContext(httpContext, new RouteData(), metadata);
					RouteValueDictionary routeValueDictionary = new RouteValueDictionary(candidates[i].Values);
					IEnumerable<IRestControllerValueProvider> services = httpContext.RequestServices.GetServices<IRestControllerValueProvider>();
					foreach (ParameterDescriptor parameter in metadata.Parameters)
					{
						foreach (IRestControllerValueProvider item in services)
						{
							object parameterValue = item.GetParameterValue(controllerContext, parameter);
							if (parameterValue != null)
							{
								routeValueDictionary[parameter.Name] = parameterValue;
								break;
							}
						}
					}
					candidates.ReplaceEndpoint(i, orAdd, routeValueDictionary);
					continue;
				}
			}
			candidates.SetValidity(i, value: false);
		}
		return Task.CompletedTask;
	}
}
