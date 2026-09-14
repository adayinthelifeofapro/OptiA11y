using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPiServer.Shell.Services.Rest;

/// <summary>
///       Represents a value provider for sort directives sent in the query string on the format sort(&lt;+/-&gt;&lt;columnName&gt;).
///       </summary>
public class SortColumnsModelBinder : IModelBinder
{
	/// <summary>
	///       Modelbinds a <see cref="T:System.Collections.Generic.IEnumerable`1" /></summary>
	/// <param name="bindingContext">The binding context</param>
	/// <returns>
	/// </returns>
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext, "bindingContext");
		string text = bindingContext.HttpContext.Request.Query.Keys.FirstOrDefault((string x) => Regex.IsMatch(x, "sort.*"));
		if (text == null)
		{
			return Task.CompletedTask;
		}
		IEnumerable<SortColumn> model = SortColumn.Parse(text);
		bindingContext.Result = ModelBindingResult.Success(model);
		return Task.CompletedTask;
	}
}
