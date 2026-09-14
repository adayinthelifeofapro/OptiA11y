using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer.Framework.Localization;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.Personalization.VisitorGroups.Internal;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc.VisitorGroups;

namespace EPiServer.VisitorGroupsCriteriaPack;

public class RoleModel : CriterionModelBase, IValidateCriterionModel
{
	private readonly Injected<IVisitorGroupRepository> _visitorGroupRepository;

	private readonly Injected<LocalizationService> _localizationService;

	private IEnumerable<VisitorGroup> _cachedVisitorGroups;

	[Required]
	[CriterionPropertyEditor(LabelTranslationKey = "/shell/cms/visitorgroups/criteria/rolecriterion/condition", Order = 1, SelectionFactoryType = typeof(EnumSelectionFactory))]
	public RoleCompareCondition Condition { get; set; }

	[Required]
	[CriterionPropertyEditor(LabelTranslationKey = "/shell/cms/visitorgroups/criteria/rolecriterion/rolename", Order = 1, SelectionFactoryType = typeof(RoleSelectionFactory))]
	public string RoleName { get; set; }

	/// <summary>
	///       Validates the specified current group.
	///       </summary>
	/// <param name="currentGroup">The current group.</param>
	/// <returns>Return CriterionValidationResult success if there is not circle reference otherwise false.</returns>
	public CriterionValidationResult Validate(VisitorGroup currentGroup)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		if (currentGroup.Id.ToString().Equals(RoleName))
		{
			return new CriterionValidationResult(false, _localizationService.Service.GetString("/shell/cms/visitorgroups/criteria/rolecriterion/validationmessage/foundcirclereference", "Bad Configuration, Found Circle Reference"));
		}
		if (FindReferenceToMe(currentGroup.Id).Any((VisitorGroup m) => m.Id.ToString() == RoleName))
		{
			return new CriterionValidationResult(false, _localizationService.Service.GetString("/shell/cms/visitorgroups/criteria/rolecriterion/validationmessage/foundcirclereference", "Bad Configuration, Found Circle Reference"));
		}
		return CriterionValidationResult.Valid;
	}

	private List<VisitorGroup> FindReferenceToMe(Guid visitorGroupId)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		List<VisitorGroup> result = new List<VisitorGroup>();
		if (_cachedVisitorGroups == null)
		{
			_cachedVisitorGroups = _visitorGroupRepository.Service.List();
		}
		FindReferenceToMe(visitorGroupId, new HashSet<Guid>(), result);
		return result;
	}

	private void FindReferenceToMe(Guid visitorGroupId, HashSet<Guid> visitedVisitorGroup, List<VisitorGroup> result)
	{
		List<VisitorGroup> list = new List<VisitorGroup>();
		list.AddRange(FindFirstLevelReferencesToMe(visitorGroupId, visitedVisitorGroup));
		visitedVisitorGroup.Add(visitorGroupId);
		result.AddRange(list);
		foreach (VisitorGroup item in list)
		{
			if (!visitedVisitorGroup.Contains(item.Id))
			{
				visitedVisitorGroup.Add(item.Id);
				FindReferenceToMe(item.Id, visitedVisitorGroup, result);
			}
		}
	}

	private List<VisitorGroup> FindFirstLevelReferencesToMe(Guid referenceVisitorGroupId, HashSet<Guid> visitedVisitorGroupsId)
	{
		List<VisitorGroup> list = new List<VisitorGroup>();
		foreach (VisitorGroup item in _cachedVisitorGroups.Where((VisitorGroup g) => !visitedVisitorGroupsId.Contains(g.Id)))
		{
			if (HasReferenceToMe(referenceVisitorGroupId, item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private static bool HasReferenceToMe(Guid referenceVisitorGroupId, VisitorGroup visitorGroup)
	{
		foreach (VisitorGroupCriterion item in visitorGroup.Criteria.Where((VisitorGroupCriterion criteria) => criteria.TypeName == VisitorGroupCriterionRepository.GetTypeName(typeof(RoleCriterion))))
		{
			if (item.Model is RoleModel roleModel && roleModel.RoleName == referenceVisitorGroupId.ToString())
			{
				return true;
			}
		}
		return false;
	}

	public override ICriterionModel Copy()
	{
		return ((CriterionModelBase)this).ShallowCopy();
	}
}
