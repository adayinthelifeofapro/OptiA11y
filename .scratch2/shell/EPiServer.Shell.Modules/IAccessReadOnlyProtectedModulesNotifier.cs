namespace EPiServer.Shell.Modules;

internal interface IAccessReadOnlyProtectedModulesNotifier
{
	bool Notify(string path);
}
