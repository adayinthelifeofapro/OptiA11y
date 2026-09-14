using System;
using System.Collections.Generic;
using EPiServer.Framework.Localization;
using EPiServer.Personalization.VisitorGroups;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EPiServer.VisitorGroupsCriteriaPack;

public class RoleCompareConditionSelectionFactory : ISelectionFactory
{
	private readonly Injected<LocalizationService> _localizationService;

	/// <summary>
	///       Gets a list of duration options
	///       </summary>
	/// <param name="propertyType">The type of the property.</param>
	/// <returns>A list of options for all categories on the site</returns>
	public IEnumerable<SelectListItem> GetSelectListItems(Type propertyType)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		return new List<SelectListItem>
		{
			new SelectListItem
			{
				Text = _localizationService.Service.GetString("/shell/cms/visitorgroups/criteria/rolecriterion/rolecomparecondition/equal"),
				Value = "Equal"
			},
			new SelectListItem
			{
				Text = _localizationService.Service.GetString("/shell/cms/visitorgroups/criteria/rolecriterion/rolecomparecondition/notequal"),
				Value = "NotEqual"
			}
		};
	}
}
