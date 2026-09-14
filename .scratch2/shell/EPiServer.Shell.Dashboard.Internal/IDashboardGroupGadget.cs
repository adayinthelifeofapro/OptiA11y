using System.Collections.Generic;

namespace EPiServer.Shell.Dashboard.Internal;

public interface IDashboardGroupGadget : IDashboardGadgetBase
{
	/// <summary>
	///       List of the gadgets in the group gadget
	///       </summary>
	IEnumerable<IDashboardGadget> Gadgets { get; set; }
}
