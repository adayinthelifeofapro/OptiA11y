using System;
using System.Collections;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using EPiServer.Framework;

namespace EPiServer.Shell.ViewComposition;

/// <summary>
///       Responsible to create a deterministic <see cref="T:System.Guid" /> for all containers and components for a view.
///       </summary>
[ViewTransformer]
public class DeterministicGuidTransformer : IViewTransformer, ISortable
{
	/// <summary>
	///       Used to select the order of execution when there are several <see cref="T:EPiServer.Shell.ViewComposition.IViewTransformer" />s.
	///       </summary>
	/// <value>
	///   <see cref="T:EPiServer.Shell.ViewComposition.DeterministicGuidTransformer" /> return 9000.</value>
	/// <remarks>This should be placed late in the view transformers list, just before <see cref="T:EPiServer.Shell.ViewComposition.PersonalizationViewTransformer" />. This means that any other transformers except <see cref="T:EPiServer.Shell.ViewComposition.PersonalizationViewTransformer" /> should have a lower value.</remarks>
	public int SortOrder => 9000;

	/// <summary>
	///       Transforms the view according to the rules for the transformer.
	///       </summary>
	/// <param name="view">The view.</param>
	/// <param name="principal">The principal.</param>
	public void TransformView(ICompositeView view, IPrincipal principal)
	{
		Guid deterministicGuid = GetDeterministicGuid(view.Name, Guid.Empty, view.RootContainer, principal.Identity.Name, 0);
		view.RootContainer.Id = deterministicGuid;
		SetDeterministicGuidRecursive(view.Name, view.RootContainer, principal.Identity.Name);
	}

	private void SetDeterministicGuidRecursive(string viewName, IContainer container, string username)
	{
		Hashtable hashtable = new Hashtable();
		foreach (IComponent component in container.Components)
		{
			int num;
			if (!hashtable.ContainsKey(component.DefinitionName))
			{
				hashtable[component.DefinitionName] = (num = 1);
			}
			else
			{
				num = (int)hashtable[component.DefinitionName];
				num++;
				hashtable[component.DefinitionName] = num;
			}
			Guid deterministicGuid = GetDeterministicGuid(viewName, container.Id, component, username, num);
			component.Id = deterministicGuid;
			if (component is IContainer container2)
			{
				SetDeterministicGuidRecursive(viewName, container2, username);
			}
		}
	}

	/// <summary>
	///       Creates a deterministic GUID based on the unique "search path" to a component.
	///       </summary>
	/// <param name="viewName">Name of the view.</param>
	/// <param name="parentId">The parents id.</param>
	/// <param name="component">The component.</param>
	/// <param name="username">The username.</param>
	/// <param name="numberOfExistingComponentsOfType">Type of the number of existing components of.</param>
	/// <returns>
	///       A deterministic GUID based on the unique "search path" to a component.
	///       </returns>
	private static Guid GetDeterministicGuid(string viewName, Guid parentId, IComponent component, string username, int numberOfExistingComponentsOfType)
	{
		string s = viewName + parentId.ToString() + component.DefinitionName + username + numberOfExistingComponentsOfType;
		byte[] array = SHA1.HashData(Encoding.UTF8.GetBytes(s));
		Array.Resize(ref array, 16);
		return new Guid(array);
	}
}
