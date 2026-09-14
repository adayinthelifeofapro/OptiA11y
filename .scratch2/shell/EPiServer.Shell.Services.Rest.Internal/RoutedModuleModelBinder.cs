using System;
using System.Threading.Tasks;
using EPiServer.Shell.Modules;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EPiServer.Shell.Services.Rest.Internal;

internal class RoutedModuleModelBinder : IModelBinder
{
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext, "bindingContext");
		object obj = bindingContext.HttpContext.Request.RouteValues["module"];
		if (obj != null && bindingContext.ModelType.IsAssignableFrom(typeof(ShellModule)))
		{
			AssignSuccessfulResult(bindingContext, obj);
		}
		return Task.CompletedTask;
	}

	private static void AssignSuccessfulResult<T>(ModelBindingContext bindingContext, T result)
	{
		bindingContext.ValidationState.Add(result, new ValidationStateEntry
		{
			SuppressValidation = true
		});
		bindingContext.Result = ModelBindingResult.Success(result);
	}
}
