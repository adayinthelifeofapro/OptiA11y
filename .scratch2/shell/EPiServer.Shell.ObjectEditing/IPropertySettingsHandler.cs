using System.Collections.Generic;
using EPiServer.Core.PropertySettings;
using EPiServer.DataAbstraction;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       Handler for property settings configured in Admin Mode
///       </summary>
public interface IPropertySettingsHandler
{
	/// <summary>
	///       Sort order for handlers. Only first handler that returns true for <see cref="M:EPiServer.Shell.ObjectEditing.IPropertySettingsHandler.CanHandle(EPiServer.DataAbstraction.PropertyDefinitionType)" /> will be used
	///       </summary>
	int SortOrder { get; }

	/// <summary>
	///       URL to client editor used in Admin Mode
	///       </summary>
	string SettingsEditor { get; }

	/// <summary>
	///       When true, then handler can be used for property definition type
	///       </summary>
	/// <param name="propertyDefinitionType">
	/// </param>
	/// <returns>
	/// </returns>
	bool CanHandle(PropertyDefinitionType propertyDefinitionType);

	/// <summary>
	///       Parse DTO from client
	///       </summary>
	/// <param name="propertySettings">
	/// </param>
	/// <param name="isListProperty">
	/// </param>
	/// <param name="errorMessage">
	/// </param>
	/// <returns>
	/// </returns>
	IEnumerable<IPropertySettings> ParseSettings(string propertySettings, bool isListProperty, out string errorMessage);

	/// <summary>
	///       Get settings stored in databse and convert to DTO
	///       </summary>
	/// <param name="settings">
	/// </param>
	/// <param name="isListProperty">
	/// </param>
	/// <returns>
	/// </returns>
	string GetSettings(IEnumerable<IPropertySettings> settings, bool isListProperty);

	/// <summary>
	///       Client editor configuration. It can contains dictionaries, default values, etc.
	///       </summary>
	/// <returns>
	/// </returns>
	object GetEditorConfiguration();
}
