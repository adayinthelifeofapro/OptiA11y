using System.Collections.Generic;
using System.Linq;
using EPiServer.Shell.Dashboard.DashboardLayoutBuilder.Internal;
using EPiServer.Shell.Dashboard.Internal;

namespace EPiServer.Shell.Dashboard.Layout.Internal;

/// <summary>
///       An element in a dashboard which contains a list of <see cref="T:EPiServer.Shell.Dashboard.Internal.IDashboardGadget" /></summary>
public class DashboardColumn
{
	private readonly List<IDashboardGadgetBase> _gadgets = new List<IDashboardGadgetBase>();

	public ColumnStyle ColumnStyle { get; set; } = ColumnStyle.FullWidth;

	public DashboardColumn()
	{
	}

	public DashboardColumn(ColumnStyle columnStyle, params IDashboardGadgetBase[] gadgets)
	{
		_gadgets = gadgets?.ToList();
		ColumnStyle = columnStyle;
	}

	public void AddGadgets(params IDashboardGadgetBase[] dashboardGadgets)
	{
		_gadgets.AddRange(dashboardGadgets);
	}

	public IEnumerable<IDashboardGadgetBase> AllGadgets()
	{
		return _gadgets;
	}
}
