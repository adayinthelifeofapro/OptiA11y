using System;
using System.Collections.Generic;

namespace EPiServer.Shell.ObjectEditing;

/// <summary>
///       NOTE: This is a pre-release API that is UNSTABLE and might not satisfy the compatibility requirements as denoted by its associated normal version.
///
///       Defines repository methods for <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> objects.
///       This is used by the headless REST api to handle editing definitions for specific combination of types and ui hints.
///       </summary>
public interface IEditorDefinitionRepository
{
	/// <summary>
	///       Adds a new <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> to the repository.
	///       </summary>
	/// <param name="editorDefinition">The <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> to add.</param>
	void Add(EditorDefinition editorDefinition);

	/// <summary>
	///       Updates an existing editor definition with the given <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.DataType" /> and <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.UIHint" /></summary>
	/// <param name="updatedValue">The <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> to update</param>
	void Update(EditorDefinition updatedValue);

	/// <summary>
	///       Loads an <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> with the specific combination (<see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.DataType" />, <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.UIHint" />).
	///       If one can't be found, it will fallback to loading with the combination (<see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.ValueType" />, <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.UIHint" />).
	///       </summary>
	/// <param name="type">The <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.DataType" /> or <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.ValueType" /> in fallback</param>
	/// <param name="uiHint">The <see cref="P:EPiServer.Shell.ObjectEditing.EditorDefinition.UIHint" /></param>
	/// <returns>A <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> that matches given <paramref name="type" /> and <paramref name="uiHint" />, or null if no match could be found.</returns>
	EditorDefinition Get(Type type, string uiHint);

	/// <summary>
	///       Deletes an <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> from the repository.
	///       </summary>
	/// <param name="editorDefinition">The <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> to delete.</param>
	void Delete(EditorDefinition editorDefinition);

	/// <summary>
	///       Lists all <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" /> in the repository. 
	///       </summary>
	/// <returns>Enumeration of <see cref="T:EPiServer.Shell.ObjectEditing.EditorDefinition" />s.</returns>
	IEnumerable<EditorDefinition> List();
}
