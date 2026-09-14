using EPiServer.Web;

namespace EPiServer.Shell.Modules.Internal;

public class ReadOnlyPageUrlResolver
{
	private readonly UIOptions _uiOptions;

	private readonly ModuleTable _moduleTable;

	private readonly IUriSupport _uriSupport;

	public ReadOnlyPageUrlResolver(UIOptions uiOptions, ModuleTable moduleTable, IUriSupport uriSupport)
	{
		_uiOptions = uiOptions;
		_moduleTable = moduleTable;
		_uriSupport = uriSupport;
	}

	public virtual string GetReadOnlyPageUrl()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		string readOnlyInfoUrl = _uiOptions.ReadOnlyInfoUrl;
		if (!string.IsNullOrWhiteSpace(readOnlyInfoUrl))
		{
			if (new UrlBuilder(readOnlyInfoUrl).HasAuthorityPart)
			{
				return readOnlyInfoUrl;
			}
			return _uriSupport.ResolveUrlBySettings(readOnlyInfoUrl);
		}
		return _moduleTable.ResolvePath("Shell", "Readonly");
	}
}
