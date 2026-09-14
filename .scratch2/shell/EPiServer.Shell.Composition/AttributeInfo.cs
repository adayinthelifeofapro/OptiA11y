using System;
using System.Diagnostics;
using System.Reflection;

namespace EPiServer.Shell.Composition;

/// <summary>
///       Encapsulates information about an attributed method.Describes an attributed method
///       </summary>
/// <typeparam name="T">Type of the decorating attribute.</typeparam>
[DebuggerDisplay("AttributedType={AttributedType}")]
internal class AttributeInfo<T>
{
	/// <summary>
	///       Gets or sets information about the attributed method. This property is null when the attribute decorates a type.
	///       </summary>
	public MethodInfo AttributedMethod { get; set; }

	/// <summary>
	///       Gets or sets information about the attributed method.
	///       </summary>
	public Type AttributedType { get; set; }

	/// <summary>
	///       Gets or sets the decorating attribute.
	///       </summary>
	public T Attribute { get; set; }
}
