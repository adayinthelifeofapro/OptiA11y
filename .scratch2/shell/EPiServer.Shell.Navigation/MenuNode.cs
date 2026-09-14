using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EPiServer.Shell.Navigation;

/// <summary>
///       Class that represents a <see cref="T:EPiServer.Shell.Navigation.MenuNode" /> and its children.
///       </summary>
public class MenuNode : IComparable<MenuNode>
{
	private List<MenuNode> _children;

	[CompilerGenerated]
	private bool _003CIsSelected_003Ek__BackingField;

	/// <summary>
	///       Parent node, Is null if it a top node
	///       </summary>
	public MenuNode Parent { get; private set; }

	/// <summary>
	///       Menu item
	///       </summary>
	public MenuItem Current { get; private set; }

	/// <summary>
	///       Child menu items
	///       </summary>
	public ICollection<MenuNode> Children => _children;

	/// <summary>
	///       Gets or sets a value indicating whether this node is selected
	///       </summary>
	/// <remarks>
	///       This will set the selection to all its parents.
	///       </remarks>
	public bool IsSelected
	{
		[CompilerGenerated]
		get
		{
			return _003CIsSelected_003Ek__BackingField;
		}
		set
		{
			if (Parent != null && (!Parent.Children.Any((MenuNode n) => n.IsSelected) & value))
			{
				Parent.IsSelected = value;
			}
			_003CIsSelected_003Ek__BackingField = value;
		}
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.MenuNode" /> class.
	///       </summary>
	/// <param name="parent">The parent menu node.</param>
	/// <param name="current">Menu item</param>
	public MenuNode(MenuNode parent, MenuItem current)
		: this(parent, current, new List<MenuNode>())
	{
	}

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Navigation.MenuNode" /> class.
	///       </summary>
	/// <param name="parent">The parent menu node.</param>
	/// <param name="current">Menu item</param>
	/// <param name="children">The immediate child nodes to this node.</param>
	public MenuNode(MenuNode parent, MenuItem current, IEnumerable<MenuNode> children)
	{
		Parent = parent;
		Current = current;
		_children = new List<MenuNode>(children);
	}

	/// <summary>
	///       Sort the list of menu items
	///       </summary>
	public void SortChildren()
	{
		_children = (from c in _children
			orderby c.Current.SortIndex, c.Current.Text
			select c).ToList();
	}

	/// <summary>
	///       Finds a <see cref="T:EPiServer.Shell.Navigation.MenuNode" />.
	///       </summary>
	/// <param name="path">The path to search for</param>
	/// <returns>Returns the <see cref="T:EPiServer.Shell.Navigation.MenuNode" /> matching the path, otherwise null.</returns>
	public MenuNode Find(string path)
	{
		ArgumentNullException.ThrowIfNull(path, "path");
		if (!path.StartsWith(Current.Path, StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}
		if (Current.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
		{
			return this;
		}
		foreach (MenuNode child in Children)
		{
			MenuNode menuNode = child.Find(path);
			if (menuNode != null)
			{
				return menuNode;
			}
		}
		return null;
	}

	/// <summary>
	///       Compares the current object with another object of the same type.
	///       The comparison is made by comparing the integer value Current.SortIndex.
	///       </summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>
	///       A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings:
	///       Value
	///       Meaning
	///       Less than zero
	///       This object is less than the <paramref name="other" /> parameter.
	///       Zero
	///       This object is equal to <paramref name="other" />.
	///       Greater than zero
	///       This object is greater than <paramref name="other" />.
	///       </returns>
	public int CompareTo(MenuNode other)
	{
		return Current.SortIndex - other.Current.SortIndex;
	}
}
