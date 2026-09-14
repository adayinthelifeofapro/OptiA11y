using System.Collections.ObjectModel;

namespace EPiServer.Shell.ObjectEditing;

internal class HandlerRegistrationSettings
{
	public Collection<IMetadataHandler> MetadataHandlers { get; private set; }

	public bool MergeBaseDescriptorsOnFirstRequest { get; set; }

	public bool BlockNewRegistrations { get; set; }

	public bool IsInitialized { get; set; }

	public HandlerRegistrationSettings()
	{
		MetadataHandlers = new Collection<IMetadataHandler>();
	}
}
