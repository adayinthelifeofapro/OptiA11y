using System.Collections.Generic;
using System.Linq;

namespace EPiServer.Shell.Dashboard.Layout.Internal;

/// <summary>
///       An element in a dashboard which contains a list of <see cref="T:EPiServer.Shell.Dashboard.Layout.Internal.DashboardColumn" /></summary>
public class DashboardRow
{
	private readonly List<DashboardColumn> _columns = new List<DashboardColumn>();

	public DashboardRow()
	{
	}

	public DashboardRow(params DashboardColumn[] columns)
	{
		_columns = columns?.ToList();
	}

	public void AddColumns(params DashboardColumn[] columns)
	{
		_columns.AddRange(columns);
	}

	public IEnumerable<DashboardColumn> AllColumns()
	{
		return _columns;
	}
}
