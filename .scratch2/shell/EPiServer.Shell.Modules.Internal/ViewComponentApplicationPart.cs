using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace EPiServer.Shell.Modules.Internal;

public class ViewComponentApplicationPart : ApplicationPart, IApplicationPartTypeProvider
{
	private readonly AssemblyName _assemblyName;

	private readonly IEnumerable<Type> _types;

	/// <summary>
	///       Gets the <see cref="P:EPiServer.Shell.Modules.Internal.ViewComponentApplicationPart.Assembly" /> of the <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPart" />.
	///       </summary>
	public Assembly Assembly { get; }

	/// <summary>
	///       Gets the name of the <see cref="T:Microsoft.AspNetCore.Mvc.ApplicationParts.ApplicationPart" />.
	///       </summary>
	public override string Name => _assemblyName.Name;

	/// <summary>
	///       Gets the ViewComponents in the assembly
	///       </summary>
	public IEnumerable<TypeInfo> Types => _types?.Where(IsViewComponent)?.Select((Type t) => t.GetTypeInfo());

	public ViewComponentApplicationPart(AssemblyName assemblyName, IEnumerable<Type> types)
	{
		_assemblyName = assemblyName ?? throw new ArgumentNullException("assemblyName");
		_types = types;
		if (_types == null)
		{
			_types = Enumerable.Empty<Type>();
		}
	}

	private bool IsViewComponent(Type type)
	{
		if (typeof(ViewComponent).IsAssignableFrom(type))
		{
			return type.Assembly.GetName().FullName == _assemblyName.FullName;
		}
		return false;
	}
}
