using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Represents a value provider for item range directives sent in the request header, for example `ItemRange=(items=0-5)`
///       </summary>
public class ItemRangeModelBinder : IModelBinder
{
	/// <summary>
	///       Model binds a <see cref="T:EPiServer.Shell.Services.Rest.ItemRange" /></summary>
	/// <param name="bindingContext">The binding context</param>
	/// <returns>
	/// </returns>
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext, "bindingContext");
		if (!bindingContext.ActionContext.ActionDescriptor.EndpointMetadata.Any((object x) => x is RestStoreAttribute))
		{
			return Task.CompletedTask;
		}
		ItemRange model = ItemRange.ReadHeaderFrom(bindingContext.HttpContext.Request) ?? new ItemRange();
		bindingContext.Result = ModelBindingResult.Success(model);
		return Task.CompletedTask;
	}
}
