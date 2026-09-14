using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EPiServer.Data.Dynamic;
using EPiServer.Framework.Cache;

namespace EPiServer.Shell.ObjectEditing.Internal;

/// <summary>
///        NOTE: This is a pre-release API that is UNSTABLE and might not satisfy the compatibility requirements as denoted by its associated normal version.
///
///        This is used by the headless REST api to handle editing definitions for specific combination of types and ui hints.
///        </summary>
internal class EditorDefinitionRepository : IEditorDefinitionRepository
{
	private readonly DynamicDataStoreFactory _ddsFactory;

	private readonly ISynchronizedObjectInstanceCache _cache;

	internal const string CacheKey = "EPi:EditorDefinitionCache";

	private static readonly Lock _cacheLock = new Lock();

	/// <summary>
	///       Initializes a new instance of the <see cref="T:EPiServer.Shell.ObjectEditing.Internal.EditorDefinitionRepository" /> class.
	///       </summary>
	/// <param name="dynamicDataStoreFactory">The factory to create the dds used.</param>
	/// <param name="cache">
	/// </param>
	public EditorDefinitionRepository(DynamicDataStoreFactory dynamicDataStoreFactory, ISynchronizedObjectInstanceCache cache)
	{
		_ddsFactory = dynamicDataStoreFactory;
		_cache = cache;
	}

	/// <summary>
	///       Adds a new editor definition to the repo.
	///       </summary>
	/// <param name="editorDefinition">
	/// </param>
	/// <returns>
	/// </returns>
	public void Add(EditorDefinition editorDefinition)
	{
		if (Get(editorDefinition.DataType, editorDefinition.UIHint) != null)
		{
			throw new InvalidOperationException($"An EditorDefinition with (DataType: {editorDefinition.DataType.FullName}, UIHint: {editorDefinition.UIHint}) already exists and cannot be added");
		}
		GetStore().Save((object)editorDefinition.ToEditorDefinitionDDSO().SerializeSettingSelections());
		ClearCache();
	}

	/// <summary>
	///       Update all fields of an editor definition except DataType and UIHint.
	///       </summary>
	/// <param name="updatedValue">
	/// </param>
	/// <returns>
	/// </returns>
	public void Update(EditorDefinition updatedValue)
	{
		EditorDefinitionDDSO editorDefinitionDDSO = GetEditorDefinitionDDSO(updatedValue.DataType, updatedValue.UIHint) ?? throw new InvalidOperationException($"An EditorDefinition with (DataType: {updatedValue.DataType.FullName}, UIHint: {updatedValue.UIHint}) does not exist and cannot be updated");
		editorDefinitionDDSO.Editor = updatedValue.Editor;
		editorDefinitionDDSO.Settings = updatedValue.Settings;
		GetStore().Save((object)editorDefinitionDDSO.SerializeSettingSelections());
		ClearCache();
	}

	/// <summary>
	///       Load an editor definition from the store for the given type and uiHint.
	///       type will first be matched to DataType and fallback to ValueType if it is not found
	///       uiHint will be set to an empty string if it is not specified or it is null
	///       </summary>
	/// <param name="type">DataType or fallback to ValueType</param>
	/// <param name="uiHint">Default value is an empty string.</param>
	/// <returns>
	/// </returns>
	public EditorDefinition Get(Type type, string uiHint = "")
	{
		return GetEditorDefinitionDDSO(type, uiHint)?.ToEditorDefinition();
	}

	/// <summary>
	///       Delete an editor definition
	///       </summary>
	/// <param name="editorDefinition">
	/// </param>
	public void Delete(EditorDefinition editorDefinition)
	{
		EditorDefinitionDDSO editorDefinitionDDSO = GetEditorDefinitionDDSO(editorDefinition.DataType, editorDefinition.UIHint) ?? throw new InvalidOperationException($"An EditorDefinition with (DataType: {editorDefinition.DataType.FullName}, UIHint: {editorDefinition.UIHint}) does not exist and cannot be deleted");
		GetStore().Delete(editorDefinitionDDSO.Id);
		ClearCache();
	}

	/// <summary>
	///       Lists all EditorDefinitions
	///       </summary>
	/// <returns>
	/// </returns>
	public IEnumerable<EditorDefinition> List()
	{
		List<EditorDefinitionDDSO> list = InternalList();
		foreach (EditorDefinitionDDSO item in list)
		{
			yield return item.ToEditorDefinition();
		}
	}

	/// <summary>
	///       Lists all EditorDefinitionDDSO
	///       </summary>
	/// <returns>
	/// </returns>
	private List<EditorDefinitionDDSO> InternalList()
	{
		List<EditorDefinitionDDSO> list = ((IObjectInstanceCache)_cache).Get("EPi:EditorDefinitionCache") as List<EditorDefinitionDDSO>;
		if (list == null)
		{
			using (_cacheLock.EnterScope())
			{
				list = ((IObjectInstanceCache)_cache).Get("EPi:EditorDefinitionCache") as List<EditorDefinitionDDSO>;
				if (list != null)
				{
					return list;
				}
				list = (from x in GetStore().LoadAll<EditorDefinitionDDSO>()
					select x.DeserializeSettingSelections()).ToList();
				((IObjectInstanceCache)_cache).Insert("EPi:EditorDefinitionCache", (object)list, (CacheEvictionPolicy)null);
			}
		}
		return list;
	}

	private DynamicDataStore GetStore()
	{
		return _ddsFactory.CreateStore(typeof(EditorDefinitionDDSO));
	}

	private EditorDefinitionDDSO GetEditorDefinitionDDSO(Type type, string uiHint = "")
	{
		ArgumentNullException.ThrowIfNull(type, "type");
		string typeString = TypeSerializer.GetAssemblyTypeNameWithoutVersion(type);
		List<EditorDefinitionDDSO> source = InternalList();
		return source.FirstOrDefault((EditorDefinitionDDSO item) => item.DataTypeString == typeString && string.Equals(item.UIHint, uiHint ?? "", StringComparison.InvariantCultureIgnoreCase)) ?? source.FirstOrDefault((EditorDefinitionDDSO item) => item.ValueTypeString == typeString && string.Equals(item.UIHint, uiHint ?? "", StringComparison.InvariantCultureIgnoreCase));
	}

	private void ClearCache()
	{
		((IObjectInstanceCache)_cache).Remove("EPi:EditorDefinitionCache");
	}
}
