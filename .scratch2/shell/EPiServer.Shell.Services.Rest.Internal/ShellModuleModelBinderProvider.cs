using System;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPiServer.Shell.Services.Rest.Internal;

public class ShellModuleModelBinderProvider : IModelBinderProvider
{
	private readonly IModelBinder _binder = new RoutedModuleModelBinder();

	/// <inheritdoc />
	public IModelBinder GetBinder(ModelBinderProviderContext context)
	{
		ArgumentNullException.ThrowIfNull(context, "context");
		if (typeof(ShellModule) == context.Metadata.ModelType)
		{
			return _binder;
		}
		return null;
	}
}
