namespace EPiServer.Shell.Dashboard.Internal;

public interface IDashboardGadget : IDashboardGadgetBase
{
	/// <summary>
	///       Url from where a gadget js module can be loaded
	///       </summary>
	string ScriptUrl { get; set; }

	/// <summary>
	///       Gadget's specific setting.
	///       All settings that need in the client side (in <see cref="P:EPiServer.Shell.Dashboard.Internal.IDashboardGadget.ScriptUrl" />) should be added in
	///       this Settings property
	///       </summary>
	object Settings { get; set; }
}
