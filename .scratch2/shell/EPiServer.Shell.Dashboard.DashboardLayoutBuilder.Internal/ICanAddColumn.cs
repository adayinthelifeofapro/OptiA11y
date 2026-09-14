using EPiServer.Shell.Dashboard.Internal;

namespace EPiServer.Shell.Dashboard.DashboardLayoutBuilder.Internal;

public interface ICanAddColumn
{
	ICanSetColumnStyle AddColumn(params IDashboardGadgetBase[] gadgets);
}
