using System;
using EPiServer.Data;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EPiServer.Shell.Modules.Internal;

public class ReadonlyDatabaseStartupFilter : IStartupFilter
{
	public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
	{
		return delegate(IApplicationBuilder app)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			ArgumentNullException.ThrowIfNull(app, "app");
			if ((int)ServiceProviderExtensions.GetInstance<IDatabaseMode>(app.ApplicationServices).DatabaseMode == 1)
			{
				app.UseMiddleware<ReadOnlyMiddleware>(Array.Empty<object>());
			}
			next(app);
		};
	}
}
