using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using EPiServer.Framework.Web.Resources;

namespace EPiServer.Shell.Configuration;

/// <summary>
///       Client resource definition
///       </summary>
[Serializable]
[XmlRoot("clientResource")]
[DebuggerDisplay("Path: {Path}, InlineContent: {InlineContent}, Index: {SortIndex}", Name = "{Name}", Type = "{ResourceType}")]
public class ClientResourceElement : IEquatable<ClientResourceElement>
{
	[CompilerGenerated]
	private ClientResourceType _003CResourceType_003Ek__BackingField;

	/// <summary>
	///       Gets or sets resource type
	///       </summary>
	[XmlAttribute("resourceType")]
	public ClientResourceType ResourceType
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CResourceType_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CResourceType_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Gets or sets the path to the resource
	///       </summary>
	/// <remarks>
	///       If the path is relative it will be autoresolved to the modules path
	///       </remarks>
	[XmlAttribute("path")]
	public string Path { get; set; }

	/// <summary>
	///       Gets or sets the client resource name.
	///       </summary>
	[XmlAttribute("name")]
	public string Name { get; set; }

	/// <summary>
	///       Gets or sets the applicable views which should use this resource.
	///       </summary>
	/// <remarks>
	///       A comma-separated list of views that the resource is applicable to. Currently, only "all" is supported.
	///       </remarks>
	[XmlAttribute("applyTo")]
	public string ApplyTo { get; set; }

	/// <summary>
	///       Obsolete, use <see cref="P:EPiServer.Shell.Configuration.ClientResourceElement.Name" /> property instead. Location stays here for backward compatibility.
	///       </summary>
	[XmlAttribute("location")]
	public string Location
	{
		get
		{
			return null;
		}
		set
		{
			if (string.IsNullOrEmpty(Name))
			{
				Name = value;
			}
		}
	}

	/// <summary>
	///       Gets or sets the sort index, use this if you have dependencies between your resources.
	///       </summary>
	[XmlAttribute("sortIndex")]
	public int SortIndex { get; set; }

	/// <summary>
	///       Gets or sets a value indicating whether this resource is already minified.
	///       </summary>
	/// <value>
	///   <c>true</c> if this resource is minified; otherwise, <c>false</c>.
	///       </value>
	[XmlAttribute("isMinified")]
	public bool IsMinified { get; set; }

	/// <summary>
	///       Gets or sets the names of other client resources that current resource depends on.
	///       </summary>
	/// <value>
	///       The dependencies.
	///       </value>
	[XmlArray("dependencies")]
	[XmlArrayItem("add")]
	public List<ClientResourceReference> Dependencies { get; set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Framework.Web.Resources.ClientResource" /> class.
	///       </summary>
	public ClientResourceElement()
		: this(string.Empty, string.Empty)
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Framework.Web.Resources.ClientResource" /> class.
	///       </summary>
	/// <param name="path">The path to the resource.</param>
	/// <param name="name">The client resource name.</param>
	public ClientResourceElement(string path, string name)
	{
		Path = path;
		Name = name;
		Dependencies = new List<ClientResourceReference>();
	}

	/// <summary>
	///       Indicates whether the current object is equal to another object of the same type.
	///       </summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>
	///       true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
	///       </returns>
	public bool Equals(ClientResourceElement other)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (ResourceType != other.ResourceType)
		{
			return false;
		}
		if (IsMinified != other.IsMinified)
		{
			return false;
		}
		if (SortIndex != other.SortIndex)
		{
			return false;
		}
		if (!Path.Equals(other.Path, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}
		List<string> first = Dependencies.Select((ClientResourceReference d) => d.Name).ToList();
		List<string> second = other.Dependencies.Select((ClientResourceReference d) => d.Name).ToList();
		return first.SequenceEqual(second);
	}

	/// <summary>
	///       Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
	///       </summary>
	/// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
	/// <returns>
	///   <c>true</c> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
	///       </returns>
	/// <exception cref="T:System.NullReferenceException">
	///       The <paramref name="obj" /> parameter is null.
	///       </exception>
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != typeof(ClientResourceElement))
		{
			return false;
		}
		return Equals((ClientResourceElement)obj);
	}

	/// <summary>
	///       Returns a hash code for this instance.
	///       </summary>
	/// <returns>
	///       A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
	///       </returns>
	public override int GetHashCode()
	{
		if (Path == null)
		{
			return 0;
		}
		return Path.GetHashCode();
	}
}
