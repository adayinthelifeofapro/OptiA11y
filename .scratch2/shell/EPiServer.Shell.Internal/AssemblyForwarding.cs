using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.VisitorGroupsCriteriaPack;

namespace EPiServer.Shell.Internal;

internal class AssemblyForwarding : IResolveType
{
	private readonly Lazy<Dictionary<string, Type>> _types;

	public AssemblyForwarding()
	{
		_types = new Lazy<Dictionary<string, Type>>(() => (from t in typeof(RoleCriterion).Assembly.GetTypes()
			where t.FullName.StartsWith("EPiServer.VisitorGroupsCriteriaPack")
			select t).ToDictionary<Type, string, Type>((Type t) => t.FullName, (Type t) => t, StringComparer.OrdinalIgnoreCase));
	}

	public Type Resolve(string typeName)
	{
		ArgumentNullException.ThrowIfNull(typeName, "typeName");
		int num = typeName.IndexOf(',');
		if (num > -1)
		{
			typeName = typeName.Substring(0, num);
		}
		_types.Value.TryGetValue(typeName, out var value);
		return value;
	}
}
