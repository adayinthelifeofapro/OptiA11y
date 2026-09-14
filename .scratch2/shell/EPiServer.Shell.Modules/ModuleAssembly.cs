using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.FileProviders;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Represents module assembly
///       </summary>
[DebuggerDisplay("{AssemblyFile.VirtualPath}, {TargetFramework}")]
public class ModuleAssembly : IEquatable<ModuleAssembly>
{
	/// <summary>
	///       Gets the collection of supported frameworks.
	///       </summary>
	public IEnumerable<FrameworkName> SupportedFrameworks
	{
		get
		{
			if (TargetFramework != null)
			{
				yield return TargetFramework;
			}
		}
	}

	/// <summary>
	///       Gets the target framework.
	///       </summary>
	public FrameworkName TargetFramework { get; private set; }

	/// <summary>
	///       Gets the assembly file.
	///       </summary>
	public IFileInfo AssemblyFile { get; private set; }

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ModuleAssembly" /> class.
	///       </summary>
	/// <param name="assemblyFile">The assembly virtual file.</param>
	/// <param name="targetFramework">The target framework.</param>
	public ModuleAssembly(IFileInfo assemblyFile, FrameworkName targetFramework)
	{
		ArgumentNullException.ThrowIfNull(assemblyFile, "assemblyFile");
		AssemblyFile = assemblyFile;
		TargetFramework = targetFramework;
	}

	/// <summary>
	///       Indicates whether the current object is equal to another object of the same type.
	///       </summary>
	/// <param name="other">An object to compare with this object.</param>
	/// <returns>
	///       true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
	///       </returns>
	public bool Equals(ModuleAssembly other)
	{
		return AssemblyFile.PhysicalPath.Equals(other.AssemblyFile.PhysicalPath);
	}

	/// <inheritdoc />
	[EditorBrowsable(EditorBrowsableState.Never)]
	public override bool Equals(object obj)
	{
		return Equals(obj as ModuleAssembly);
	}

	/// <summary>
	///       Returns a hash code for this instance.
	///       </summary>
	/// <returns>
	///       A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
	///       </returns>
	public override int GetHashCode()
	{
		return AssemblyFile.PhysicalPath.GetHashCode();
	}
}
