using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EPiServer.Framework.TypeScanner;

namespace EPiServer.Shell.Composition;

internal static class AttributeDiscovery
{
	/// <summary>
	///       Probes the assemblies for classes decorated with an attribute.
	///       </summary>
	/// <param name="assemblies">The list of assemblies to search</param>
	/// <param name="typeScannerRepository">The global type scanner repository</param>
	/// <param name="classBaseTypes">The base types required</param>
	/// <typeparam name="TAttribute">The attribute to look for.</typeparam>
	public static IEnumerable<AttributeInfo<TAttribute>> GetAttributedTypes<TAttribute>(this IEnumerable<Assembly> assemblies, ITypeScannerLookup typeScannerRepository, params Type[] classBaseTypes) where TAttribute : Attribute
	{
		ArgumentNullException.ThrowIfNull(assemblies, "assemblies");
		ArgumentNullException.ThrowIfNull(classBaseTypes, "classBaseTypes");
		ArgumentNullException.ThrowIfNull(typeScannerRepository, "typeScannerRepository");
		IEnumerable<Type> enumerable = from assemblyType in typeScannerRepository.AllTypes
			where assemblies.Any((Assembly a) => assemblyType.Assembly == a)
			where classBaseTypes.Any((Type baseType) => baseType.IsAssignableFrom(assemblyType))
			select assemblyType;
		List<AttributeInfo<TAttribute>> list = new List<AttributeInfo<TAttribute>>();
		foreach (Type item2 in enumerable)
		{
			foreach (TAttribute customAttribute in item2.GetCustomAttributes<TAttribute>(inherit: true))
			{
				AttributeInfo<TAttribute> item = new AttributeInfo<TAttribute>
				{
					AttributedType = item2,
					Attribute = customAttribute
				};
				list.Add(item);
			}
		}
		return list;
	}

	/// <summary>
	///       Probes assemblies for methods decorated with the specified attribute.
	///       </summary>
	/// <typeparam name="TAttribute">The type of the attribute to probe.</typeparam>
	/// <param name="assemblies">The assemblies to probe.</param>
	/// <param name="typeScannerRepository">The global type scanner repository</param>
	/// <param name="classBaseTypes">Type of the base class.</param>
	/// <returns>
	/// </returns>
	public static IEnumerable<AttributeInfo<TAttribute>> GetAttributedMethods<TAttribute>(this IEnumerable<Assembly> assemblies, ITypeScannerLookup typeScannerRepository, params Type[] classBaseTypes) where TAttribute : Attribute
	{
		ArgumentNullException.ThrowIfNull(assemblies, "assemblies");
		ArgumentNullException.ThrowIfNull(classBaseTypes, "classBaseTypes");
		ArgumentNullException.ThrowIfNull(typeScannerRepository, "typeScannerRepository");
		IEnumerable<Type> enumerable = from assemblyType in typeScannerRepository.AllTypes
			where assemblies.Any((Assembly a) => assemblyType.Assembly == a)
			where classBaseTypes.Any((Type baseType) => baseType.IsAssignableFrom(assemblyType))
			select assemblyType;
		List<AttributeInfo<TAttribute>> list = new List<AttributeInfo<TAttribute>>();
		foreach (Type item in enumerable)
		{
			MethodInfo[] methods = item.GetMethods(BindingFlags.Instance | BindingFlags.Public);
			foreach (MethodInfo methodInfo in methods)
			{
				foreach (TAttribute customAttribute in methodInfo.GetCustomAttributes<TAttribute>(inherit: false))
				{
					list.Add(new AttributeInfo<TAttribute>
					{
						AttributedType = item,
						AttributedMethod = methodInfo,
						Attribute = customAttribute
					});
				}
			}
		}
		return list;
	}
}
