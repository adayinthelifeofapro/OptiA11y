namespace EPiServer.Shell.Dashboard.DashboardLayoutBuilder.Internal;

public interface ICanSetColumnStyle
{
	/// <summary>
	///       Set column width
	///       </summary>
	/// <param name="columnStyle">
	/// </param>
	/// <returns>
	/// </returns>
	ICanBuild SetColumnStyle(ColumnStyle columnStyle);
}
