using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Framework;
using EPiServer.Framework.TypeScanner;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Provides components that are defined in a <see cref="T:EPiServer.Shell.ViewComposition.ComponentAttribute" />.
///       </summary>
[ComponentProvider]
public class AttributedComponentProvider : IComponentProvider, ISortable
{
	private readonly ITypeScannerLookup _typeScannerLookup;

	private IEnumerable<IComponentDefinition> _componentDefinitions;

	/// <summary>
	///       Used to select the order of execution of the <see cref="T:EPiServer.Shell.ViewComposition.IComponentProvider" />s.
	///       </summary>
	/// <value>
	///   <see cref="T:EPiServer.Shell.ViewComposition.AttributedComponentProvider" /> has a sort order of 150.</value>
	public int SortOrder => 150;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ViewComposition.AttributedComponentProvider" /> class.
	///       </summary>
	/// <param name="typeLookup">The type repository providing types scanned for attributes.</param>
	public AttributedComponentProvider(ITypeScannerLookup typeLookup)
	{
		_typeScannerLookup = typeLookup;
	}

	/// <summary>
	///       Returns all components provided by this provider.
	///       </summary>
	/// <returns>
	/// </returns>
	public IEnumerable<IComponentDefinition> GetComponentDefinitions()
	{
		if (_componentDefinitions == null)
		{
			var source = _typeScannerLookup.AllTypes.SelectMany((Type t) => from a in t.GetCustomAttributes<ComponentAttribute>(inherit: true)
				select new
				{
					Type = t,
					Attribute = a
				});
			_componentDefinitions = source.Select(cat => cat.Attribute.CreateComponentDefinition(cat.Type)).ToList();
		}
		return _componentDefinitions;
	}

	/// <summary>
	///       Try creating a component instance corresponding to an <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" />.
	///       </summary>
	/// <param name="definition">The component definition to create a component for.</param>
	/// <returns>A component instance if the definition is provided by this provider; otherwise <c>null</c></returns>
	public IComponent CreateComponent(IComponentDefinition definition)
	{
		if (GetComponentDefinitions().Contains(definition))
		{
			return definition.CreateComponent();
		}
		return null;
	}
}
