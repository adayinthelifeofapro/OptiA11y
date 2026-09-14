using System.Collections.Generic;
using EPiServer.Shell.Dashboard.Internal;
using EPiServer.Shell.Dashboard.Layout.Internal;

namespace EPiServer.Shell.Dashboard.DashboardLayoutBuilder.Internal;

public class DashboardBuilder : ICanAddRow, ICanAddColumn, ICanSetColumnStyle, ICanBuild
{
	private readonly List<DashboardRow> _dashboardRows = new List<DashboardRow>();

	private DashboardRow _activeRow;

	private DashboardColumn _activeColumn;

	private DashboardBuilder()
	{
	}

	public static ICanAddRow Create()
	{
		return new DashboardBuilder();
	}

	public ICanAddColumn AddRow()
	{
		_activeRow = new DashboardRow();
		_dashboardRows.Add(_activeRow);
		return this;
	}

	public ICanSetColumnStyle AddColumn(params IDashboardGadgetBase[] gadgets)
	{
		_activeColumn = new DashboardColumn();
		_activeColumn.AddGadgets(gadgets);
		_activeRow.AddColumns(_activeColumn);
		return this;
	}

	public ICanBuild SetColumnStyle(ColumnStyle columnStyle)
	{
		_activeColumn.ColumnStyle = columnStyle;
		return this;
	}

	public IList<DashboardRow> Build()
	{
		return _dashboardRows;
	}
}
