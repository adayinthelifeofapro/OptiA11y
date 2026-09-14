using System.Collections.Generic;
using EPiServer.Shell.Dashboard.Layout.Internal;

namespace EPiServer.Shell.Dashboard.DashboardLayoutBuilder.Internal;

public interface ICanBuild : ICanAddColumn, ICanAddRow
{
	IList<DashboardRow> Build();
}
