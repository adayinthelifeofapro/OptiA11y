using System;
using System.Security.Principal;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EPiServer.VisitorGroupsCriteriaPack;

/// <summary>
///       Implementation of a EPiServer.Personalization.VisitorGroups.CriterionBase
///       which checks if a user is in a named role.
///       </summary>
[VisitorGroupCriterion(Category = "Technical Criteria", DisplayName = "Role", Description = "Criterion that matches the user's roles", DisableServiceRegistration = true)]
public class RoleCriterion : CriterionBase<RoleModel>
{
	private readonly Injected<ILogger<RoleCriterion>> _logger;

	private readonly Injected<IVisitorGroupRepository> _visitorGroupRepository;

	public override bool IsMatch(IPrincipal principal, HttpContext httpContext)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (principal == null)
		{
			return false;
		}
		string text = string.Empty;
		bool flag = false;
		Guid result;
		if (base.Model.RoleName.StartsWith("__"))
		{
			text = base.Model.RoleName.Replace("__ROLE__", string.Empty).Replace("__VR__", string.Empty);
			flag = principal.IsInRole(text);
		}
		else if (Guid.TryParse(base.Model.RoleName, out result))
		{
			VisitorGroup val = _visitorGroupRepository.Service.Load(result);
			if (val != null)
			{
				text = val.Name;
				flag = IPrincipalExtensions.IsInRole(principal, text, (SecurityEntityType)2);
			}
		}
		_logger.Service.TargetRole(text);
		_logger.Service.TestingUser(principal.Identity.Name);
		bool flag2 = base.Model.Condition == RoleCompareCondition.Equal;
		return flag == flag2;
	}
}
