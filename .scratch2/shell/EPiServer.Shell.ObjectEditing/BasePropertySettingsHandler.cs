using System.Collections.Generic;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;

namespace EPiServer.Shell.ObjectEditing;

public abstract class BasePropertySettingsHandler : IPropertySettingsHandler
{
	public virtual int SortOrder => 100;

	public abstract string SettingsEditor { get; }

	public abstract bool CanHandle(PropertyDefinitionType propertyDefinitionType);

	public abstract IEnumerable<IPropertySettings> ParseSettings(string propertySettings, bool isListProperty, out string errorMessage);

	public abstract string GetSettings(IEnumerable<IPropertySettings> settings, bool isListProperty);

	public virtual object GetEditorConfiguration()
	{
		return null;
	}
}
