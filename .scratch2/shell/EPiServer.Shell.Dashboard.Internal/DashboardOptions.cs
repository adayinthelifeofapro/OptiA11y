using System.Collections.Generic;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Dashboard.Layout.Internal;

namespace EPiServer.Shell.Dashboard.Internal;

/// <summary>
///       Option class to configure the dashboard - which gadgets shoud be added, which layout it should have.
///       </summary>
[Options(ConfigurationSection = "CmsUI")]
public class DashboardOptions
{
	public IList<DashboardRow> DashboardRows { get; set; } = new List<DashboardRow>();
}
