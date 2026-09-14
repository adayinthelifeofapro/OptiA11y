using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace EPiServer.Shell.Modules;

internal class DefaultAccessReadOnlyProtectedModulesNotifier : IAccessReadOnlyProtectedModulesNotifier, IAccessReadOnlyProtectedModules
{
	private readonly ILogger<DefaultAccessReadOnlyProtectedModulesNotifier> _log;

	public event EventHandler<ReadOnlyProtectedModuleEventArgs> AccessPath;

	public DefaultAccessReadOnlyProtectedModulesNotifier(ILogger<DefaultAccessReadOnlyProtectedModulesNotifier> logger)
	{
		_log = logger;
	}

	public bool Notify(string path)
	{
		EventHandler<ReadOnlyProtectedModuleEventArgs> eventHandler = AccessPath;
		if (eventHandler == null)
		{
			return false;
		}
		ReadOnlyProtectedModuleEventArgs e = new ReadOnlyProtectedModuleEventArgs(path);
		foreach (EventHandler<ReadOnlyProtectedModuleEventArgs> item in eventHandler.GetInvocationList().OfType<EventHandler<ReadOnlyProtectedModuleEventArgs>>())
		{
			item(this, e);
			if (e.Handled)
			{
				_log.AccessReadOnlyProtectedModuleHandled(item?.Method);
				return true;
			}
		}
		return e.Handled;
	}
}
