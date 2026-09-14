using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Shell.Security.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.VisitorGroupsCriteriaPack;

public class RoleSelectionFactory : ISelectionFactory
{
	private Injected<IVisitorGroupRepository> _visitorGroupRepository;

	private Injected<IVirtualRoleRepository> _virtualRoleRepository;

	private Injected<SecurityConfiguration> _securityConfiguration;

	public const string Prefix = "__";

	public const string RolePrefix = "__ROLE__";

	public const string VirtualRolePrefix = "__VR__";

	[Obsolete("Replaced with 'Prefix'.", true)]
	public const string PREFIX = "__";

	[Obsolete("Replaced with 'RolePrefix'.", true)]
	public const string ROLE_PREFIX = "__ROLE__";

	[Obsolete("Replaced with 'VirtualRolePrefix'.", true)]
	public const string VIRTUALROLE_PREFIX = "__VR__";

	IEnumerable<SelectListItem> ISelectionFactory.GetSelectListItems(Type propertyType)
	{
		List<SelectListItem> list = (from vg in _visitorGroupRepository.Service.List()
			where vg.IsSecurityRole
			select new SelectListItem
			{
				Text = vg.Name,
				Value = vg.Id.ToString()
			}).ToList();
		IEnumerable<SelectListItem> second = (_securityConfiguration.Service.IsRoleManagementEnabled ? _securityConfiguration.Service.UiRoleProvider.GetAllRolesAsync().ToBlockingEnumerable() : Enumerable.Empty<IUIRole>()).Select((IUIRole item) => new SelectListItem
		{
			Text = item.Name,
			Value = "__ROLE__" + item.Name
		});
		List<string> namesOfSecurityRolesVg = list.Select((SelectListItem vg) => vg.Text).ToList();
		IEnumerable<SelectListItem> second2 = from vrName in _virtualRoleRepository.Service.GetAllRoles().Distinct().TakeWhile((string vrName) => !namesOfSecurityRolesVg.Contains(vrName))
			select new SelectListItem
			{
				Text = vrName,
				Value = "__VR__" + vrName
			};
		return list.Union(second).Union(second2);
	}
}
