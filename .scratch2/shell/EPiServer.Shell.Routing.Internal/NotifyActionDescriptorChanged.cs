using System.Threading;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace EPiServer.Shell.Routing.Internal;

public class NotifyActionDescriptorChanged : IActionDescriptorChangeProvider
{
	private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

	private CancellationChangeToken _changeToken;

	public NotifyActionDescriptorChanged()
	{
		_changeToken = new CancellationChangeToken(_cancellationTokenSource.Token);
	}

	public void SignalChanged()
	{
		_cancellationTokenSource.Cancel();
	}

	public IChangeToken GetChangeToken()
	{
		if (_cancellationTokenSource.IsCancellationRequested)
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_changeToken = new CancellationChangeToken(_cancellationTokenSource.Token);
		}
		return _changeToken;
	}
}
