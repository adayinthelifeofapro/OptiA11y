using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace EPiServer.Shell.Services.Rest.Internal;

public class EntityModelBinderProvider : IModelBinderProvider
{
	private readonly IModelBinder _binder = new EntityModelBinder();

	internal static readonly string ParameterName = "entity";

	/// <inheritdoc />
	public IModelBinder GetBinder(ModelBinderProviderContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		if (ParameterName.Equals(context.Metadata.Name) && context.Services.GetService<IHttpContextAccessor>().HttpContext?.Request.RouteValues[ParameterName] != null)
		{
			return _binder;
		}
		return null;
	}
}
