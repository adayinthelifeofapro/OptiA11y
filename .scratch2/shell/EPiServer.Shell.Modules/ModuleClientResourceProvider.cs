using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EPiServer.Framework.Web.Resources;
using EPiServer.Shell.Configuration;

namespace EPiServer.Shell.Modules;

/// <summary>
///       Module client resource provider
///       </summary>
internal class ModuleClientResourceProvider : IClientResourceProvider
{
	private readonly ModuleTable _modules;

	private readonly Lazy<List<ClientResource>> _clientResources;

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.Modules.ModuleClientResourceProvider" /> class.
	///       </summary>
	/// <param name="modules">The module manager.</param>
	public ModuleClientResourceProvider(ModuleTable modules)
	{
		_modules = modules;
		_clientResources = new Lazy<List<ClientResource>>(LoadClientResources, LazyThreadSafetyMode.PublicationOnly);
	}

	/// <summary>
	///       Gets the client resoures specified inside the <see cref="T:EPiServer.Shell.Configuration.ShellModuleManifest" /> file fo public modules.
	///       </summary>
	/// <returns>A list of client resources resolved for public modules.</returns>
	public IEnumerable<ClientResource> GetClientResources()
	{
		return _clientResources.Value;
	}

	private List<ClientResource> LoadClientResources()
	{
		List<ClientResource> list = new List<ClientResource>();
		foreach (ShellModule module in _modules.GetModules())
		{
			IEnumerable<ClientResource> collection = ((IEnumerable<ClientResourceElement>)module.Manifest.ClientResources).Select((Func<ClientResourceElement, ClientResource>)delegate(ClientResourceElement r)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_001d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0102: Expected O, but got Unknown
				return new ClientResource
				{
					ResourceType = r.ResourceType,
					Name = r.Name,
					Path = ((!string.IsNullOrWhiteSpace(r.Path) && Uri.IsWellFormedUriString(r.Path, UriKind.Absolute)) ? r.Path : _modules.ResolveClientPath(module.Name, r.Path)),
					SortIndex = r.SortIndex,
					ApplyTo = (string.IsNullOrEmpty(r.ApplyTo) ? new List<string>() : (from s in r.ApplyTo.ToLowerInvariant().Split(",", StringSplitOptions.RemoveEmptyEntries)
						select s.Trim()).ToList()),
					Dependencies = r.Dependencies.Select((ClientResourceReference d) => d.Name).ToList()
				};
			});
			list.AddRange(collection);
		}
		return list;
	}
}
