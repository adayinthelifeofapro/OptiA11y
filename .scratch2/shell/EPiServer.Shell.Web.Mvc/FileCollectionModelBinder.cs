using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EPiServer.Shell.Web.Mvc;

/// <summary>
///       Provides binder for the files collection.
///       </summary>
public class FileCollectionModelBinder : IModelBinder
{
	/// <inheritdoc />
	public Task BindModelAsync(ModelBindingContext bindingContext)
	{
		ArgumentNullException.ThrowIfNull(bindingContext, "bindingContext");
		List<IFormFile> list = new List<IFormFile>();
		foreach (IFormFile file in bindingContext.HttpContext.Request.Form.Files)
		{
			string name = file.Name;
			if (IsModelNameMatchInputName(bindingContext.ModelName, name) && !string.IsNullOrEmpty(file.FileName))
			{
				list.Add(file);
			}
		}
		bindingContext.Result = ModelBindingResult.Success(list);
		return Task.CompletedTask;
	}

	private bool IsModelNameMatchInputName(string modelName, string inputName)
	{
		string text;
		if (!modelName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
		{
			text = modelName;
		}
		else
		{
			text = modelName.Substring(0, modelName.Length - 1);
		}
		string value = text;
		return inputName.StartsWith(value, StringComparison.OrdinalIgnoreCase);
	}
}
