using System;
using System.Collections.Generic;

namespace EPiServer.Shell.Storage;

/// <summary>
///       Class used for storing component configuration to database.
///       This class supports the EPiServer infrastructure and is not intended to be used directly from your code.
///       </summary>
public class ComponentData
{
	private Guid _id = Guid.NewGuid();

	/// <summary>
	///       Gets or sets the unique id of the component instance.
	///       </summary>
	public virtual Guid Id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	/// <summary>
	///       Name of the <see cref="T:EPiServer.Shell.ViewComposition.IComponentDefinition" /> this component is instantiated from.
	///       </summary>
	public string DefinitionName { get; set; }

	/// <summary>
	///       Gets or sets the plug in area where this container appears.
	///       </summary>
	/// <value>The plug in path.</value>
	public virtual string PlugInArea { get; set; }

	/// <summary>
	///       The generic settings collection persisted for each component.
	///       </summary>
	/// <value>The settings.</value>
	public IDictionary<string, object> Settings { get; set; }

	/// <summary>
	///       The components configured in this container.
	///       </summary>
	public IList<ComponentData> Components { get; protected set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Storage.ComponentData" /> class.
	///       </summary>
	public ComponentData()
	{
		Settings = new Dictionary<string, object>();
		Components = new List<ComponentData>();
	}
}
