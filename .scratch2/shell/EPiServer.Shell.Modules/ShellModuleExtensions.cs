namespace EPiServer.Shell.Modules;

internal static class ShellModuleExtensions
{
	public static bool IsAppModule(this ShellModule shellModule)
	{
		return "App".Equals(shellModule.Name);
	}
}
