using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Security;
using EPiServer.Shell.Profile;
using EPiServer.Shell.Profile.Internal;
using EPiServer.Shell.Security;
using EPiServer.Shell.Security.Internal;
using EPiServer.Shell.ViewComposition;

namespace EPiServer.Shell.UserMembership.Internal;

public class UserMembershipService : IUserMembershipService
{
	private const string TouchDisabledProfileKey = "epi-touchdisabled";

	private const string LanguageProfileKey = "epi-language";

	private const string HeadlessModeEnabledKey = "epi-headlessmodeenabled";

	private readonly IProfileRepository _profileRepository;

	private readonly LocalizationService _localizationService;

	private readonly ICurrentUiCulture _currentUiCulture;

	private readonly IUserImpersonation _userImpersonation;

	private readonly IPersonalizedViewSettingsRepository _personalizedViewSettingsRepository;

	private readonly SecurityConfiguration _securityConfiguration;

	public UserMembershipService(IProfileRepository profileRepository, LocalizationService localizationService, ICurrentUiCulture currentUiCulture, IUserImpersonation userImpersonation, IPersonalizedViewSettingsRepository personalizedViewSettingsRepository, SecurityConfiguration securityConfiguration)
	{
		_profileRepository = profileRepository;
		_localizationService = localizationService;
		_currentUiCulture = currentUiCulture;
		_userImpersonation = userImpersonation;
		_personalizedViewSettingsRepository = personalizedViewSettingsRepository;
		_securityConfiguration = securityConfiguration;
	}

	public async Task<UserMembershipInfo> GetUser(string userName)
	{
		UserMembershipInfo memberShipInfo = new UserMembershipInfo
		{
			UserName = userName
		};
		IUIUser user = await _securityConfiguration.GetUserAsync(userName);
		if (user != null)
		{
			memberShipInfo.UserName = user.Username;
			memberShipInfo.Email = user.Email;
			memberShipInfo.RequiresQuestionAndAnswer = _securityConfiguration.UiUserProvider?.RequiresQuestionAndAnswer ?? false;
			memberShipInfo.PasswordQuestion = user.PasswordQuestion;
			UserMembershipInfo userMembershipInfo = memberShipInfo;
			userMembershipInfo.Roles = await GetMembershipUserRolesAsync(user.Username).ToListAsync();
			userMembershipInfo = memberShipInfo;
			userMembershipInfo.AllRoles = await GetAllRoles().ConfigureAwait(continueOnCapturedContext: false);
			memberShipInfo.IsApproved = user.IsApproved;
			memberShipInfo.IsLockedOut = user.IsLockedOut;
			memberShipInfo.CreationDate = user.CreationDate;
			memberShipInfo.LastLoginDate = user.LastLoginDate;
			memberShipInfo.ProviderName = user.ProviderName;
		}
		memberShipInfo.IsReadOnly = user == null || !_securityConfiguration.IsRoleManagementEnabled;
		return memberShipInfo;
	}

	public async Task<IEnumerable<string>> GetAllRoles()
	{
		if (!_securityConfiguration.IsRoleManagementEnabled)
		{
			return Enumerable.Empty<string>();
		}
		return await (from x in _securityConfiguration.UiRoleProvider.GetAllRolesAsync()
			select x.Name).ToListAsync();
	}

	public async Task<UserSaveResult> CreateUser(UserMembershipSaveInfo model)
	{
		if (await _securityConfiguration.GetUserAsync(model.UserName) != null)
		{
			return new UserSaveResult
			{
				Status = UIUserCreateStatus.DuplicateUserName
			};
		}
		if (!ValidateEmail(model.Email))
		{
			return new UserSaveResult
			{
				Status = UIUserCreateStatus.InvalidEmail
			};
		}
		string passwordQuestion = (_securityConfiguration.UiUserProvider.RequiresQuestionAndAnswer ? model.PasswordQuestion : null);
		string passwordAnswer = (_securityConfiguration.UiUserProvider.RequiresQuestionAndAnswer ? model.PasswordAnswer : null);
		CreateUserResult createUserResult = await _securityConfiguration.UiUserProvider.CreateUserAsync(model.UserName, model.Password, model.Email, passwordQuestion, passwordAnswer, model.IsApproved);
		UserSaveResult userSaveResult = new UserSaveResult
		{
			Status = createUserResult.Status,
			Errors = createUserResult.Errors
		};
		if (userSaveResult.Status != UIUserCreateStatus.Success)
		{
			return userSaveResult;
		}
		return await SaveRolesAsync(userSaveResult, null, model);
	}

	public async Task<UserSaveResult> SaveUser(UserMembershipSaveInfo model)
	{
		IUIUser membershipUser = (await _securityConfiguration.GetUserAsync(model.UserName)) ?? throw new EPiServerException("User to update not found");
		return await SaveRolesAsync(await SaveMembershipUserAsync(membershipUser, model), membershipUser, model);
	}

	private async Task<UserSaveResult> SaveRolesAsync(UserSaveResult status, IUIUser membershipUser, UserMembershipSaveInfo model)
	{
		if (status.Status == UIUserCreateStatus.Success)
		{
			await SaveMembershipRolesAsync(model, membershipUser).ConfigureAwait(continueOnCapturedContext: false);
			return status;
		}
		return status;
	}

	public async Task<bool> ResetViews(string userName)
	{
		IPrincipal principal = await _userImpersonation.CreatePrincipalAsync(userName);
		foreach (PersonalizedViewSettings item in _personalizedViewSettingsRepository.Load(principal))
		{
			_personalizedViewSettingsRepository.Delete(principal, item.ViewName);
		}
		return true;
	}

	/// <summary>
	///       Saves the MembershipUser Roles. This is made by removing all existing roles from user
	///       and add those selected. This may be more expensive for the database but easier in code
	///       instead of trying to differ previously added Roles to current selected.
	///       </summary>
	private async Task<bool> SaveMembershipRolesAsync(UserMembershipSaveInfo model, IUIUser membershipUser)
	{
		if (model.IsAdminMode)
		{
			if (membershipUser != null)
			{
				List<string> list = await _securityConfiguration.UiRoleProvider.GetRolesForUserAsync(model.UserName).ToListAsync();
				if (list.Count > 0)
				{
					await _securityConfiguration.UiRoleProvider.RemoveUserFromRolesAsync(model.UserName, list).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			IEnumerable<string> roles = model.Roles;
			if (roles != null && roles.Any())
			{
				await _securityConfiguration.UiRoleProvider.AddUserToRolesAsync(model.UserName, roles).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		return true;
	}

	/// <summary>
	///       Gets all Roles which the membership user is added to. If the membership username
	///       isn't found in HTTP Request an empty hashtable is returned.
	///       </summary>
	/// <returns>
	/// </returns>
	private IAsyncEnumerable<string> GetMembershipUserRolesAsync(string userName)
	{
		if (!string.IsNullOrEmpty(userName) && _securityConfiguration.IsRoleManagementEnabled)
		{
			return _securityConfiguration.UiRoleProvider.GetRolesForUserAsync(userName);
		}
		return AsyncEnumerable.Empty<string>();
	}

	private async Task<UserSaveResult> SaveMembershipUserAsync(IUIUser membershipUser, UserMembershipSaveInfo model)
	{
		if (_securityConfiguration.UiUserManager == null)
		{
			return new UserSaveResult
			{
				Status = UIUserCreateStatus.Success
			};
		}
		if (model.ChangeEmail)
		{
			bool flag = string.IsNullOrEmpty(model.CurrentPassword);
			if (!flag)
			{
				flag = !(await _securityConfiguration.UiUserManager.CheckPasswordAsync(membershipUser, model.CurrentPassword));
			}
			if (flag)
			{
				return new UserSaveResult
				{
					Status = UIUserCreateStatus.IncorrectCurrentPassword
				};
			}
		}
		if (!ValidateEmail(model.Email))
		{
			return new UserSaveResult
			{
				Status = UIUserCreateStatus.InvalidEmail
			};
		}
		string pwdAnswer = (_securityConfiguration.UiUserProvider.RequiresQuestionAndAnswer ? model.PasswordAnswer : null);
		if (model.ChangePassword)
		{
			if (model.CurrentPassword == model.Password)
			{
				return new UserSaveResult
				{
					Status = UIUserCreateStatus.SamePassword
				};
			}
			if (!ValidatePassword(model))
			{
				return new UserSaveResult
				{
					Status = UIUserCreateStatus.InvalidPassword
				};
			}
			if (!model.IsAdminMode)
			{
				await _securityConfiguration.UiUserManager.ChangePasswordAsync(membershipUser, model.CurrentPassword, model.Password);
			}
			else
			{
				await _securityConfiguration.UiUserManager.ResetPasswordAsync(membershipUser, model.Password);
			}
			await _securityConfiguration.UiUserManager.ChangePasswordQuestionAndAnswerAsync(membershipUser, model.Password, membershipUser.PasswordQuestion, pwdAnswer);
		}
		if ((model.ChangeEmail || model.IsAdminMode) && _securityConfiguration.UiUserProvider.IsSupported(membershipUser.ProviderName, "email"))
		{
			membershipUser.Email = model.Email.Trim();
		}
		if (model.IsAdminMode)
		{
			if (!model.IsLockedOut && membershipUser.IsLockedOut)
			{
				await _securityConfiguration.UiUserManager.UnlockUserAsync(membershipUser);
			}
			membershipUser.IsApproved = model.IsApproved;
			membershipUser.IsLockedOut = model.IsLockedOut;
		}
		UIUserActionResult uIUserActionResult = await _securityConfiguration.UiUserProvider.UpdateUserAsync(membershipUser);
		if (uIUserActionResult.Status)
		{
			return new UserSaveResult
			{
				Status = UIUserCreateStatus.Success,
				Errors = null
			};
		}
		return new UserSaveResult
		{
			Status = UIUserCreateStatus.ProviderError,
			Errors = uIUserActionResult.Errors
		};
	}

	private string Translate(string key)
	{
		return _localizationService.GetString(key);
	}

	public string GetMessageFromStatus(UIUserCreateStatus status, string userName, IEnumerable<string> errors)
	{
		return status switch
		{
			UIUserCreateStatus.DuplicateEmail => Translate("/admin/edituser/duplicateemailaddress"), 
			UIUserCreateStatus.DuplicateProviderUserKey => Translate("/admin/edituser/duplicateprovideruserkey"), 
			UIUserCreateStatus.DuplicateUserName => string.Format(Translate("/admin/secedit/usernameexists"), userName), 
			UIUserCreateStatus.InvalidAnswer => Translate("/admin/edituser/invalidpasswordanswer"), 
			UIUserCreateStatus.InvalidEmail => Translate("/admin/edituser/invalidemail"), 
			UIUserCreateStatus.InvalidPassword => string.Format(Translate("/admin/edituser/invalidpassword"), _securityConfiguration.UiUserProvider.MinRequiredPasswordLength, _securityConfiguration.UiUserProvider.MinRequiredNonAlphanumericCharacters), 
			UIUserCreateStatus.InvalidProviderUserKey => Translate("/admin/edituser/invalidprovideruserkey"), 
			UIUserCreateStatus.InvalidQuestion => Translate("/admin/edituser/invalidpasswordquestion"), 
			UIUserCreateStatus.InvalidUserName => Translate("/admin/edituser/invalidusername"), 
			UIUserCreateStatus.ProviderError => Translate("/admin/edituser/providererror") + "-" + string.Join("-", errors ?? Enumerable.Empty<string>()), 
			UIUserCreateStatus.UserRejected => Translate("/admin/edituser/userrejected"), 
			UIUserCreateStatus.SamePassword => Translate("/admin/edituser/samepassword"), 
			UIUserCreateStatus.IncorrectCurrentPassword => Translate("/admin/edituser/incorrectcurrentpassword"), 
			_ => Translate("/admin/edituser/usersuccess"), 
		};
	}

	public void SaveDisplayOptions(string userName, DisplayOptions model)
	{
		_currentUiCulture.Save(userName, CultureInfo.GetCultureInfo(model.Language ?? ""));
		ProfileData orCreateProfile = _profileRepository.GetOrCreateProfile(userName);
		orCreateProfile.Settings["epi-language"] = model.Language;
		orCreateProfile.Settings["epi-headlessmodeenabled"] = model.HeadlessModeEnabled;
		orCreateProfile.Settings["epi-touchdisabled"] = model.TouchSupport;
		_profileRepository.Save(orCreateProfile);
	}

	private bool GetTouchSupport(ProfileData profile)
	{
		if (profile.Settings.TryGetValue("epi-touchdisabled", out var value) && bool.TryParse(value.ToString(), out var result))
		{
			return result;
		}
		return false;
	}

	private bool GetHeadlessModeEnabled(ProfileData profile)
	{
		if (profile.Settings.TryGetValue("epi-headlessmodeenabled", out var value) && bool.TryParse(value.ToString(), out var result))
		{
			return result;
		}
		return false;
	}

	private static string GetUserPreferredLanguage(ProfileData profile)
	{
		if (profile.Settings.TryGetValue("epi-language", out var value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public IEnumerable<DisplayLanguage> GetAvailableLanguages()
	{
		yield return new DisplayLanguage
		{
			Label = _localizationService.GetString("/admin/edituser/userguisettings/settings/usesystemlanguage"),
			Value = string.Empty
		};
		foreach (CultureInfo availableLocalization in _localizationService.AvailableLocalizations)
		{
			StringBuilder stringBuilder = new StringBuilder(availableLocalization.NativeName);
			if (availableLocalization.TextInfo.ANSICodePage != 1252)
			{
				stringBuilder.AppendFormat(" ({0})", availableLocalization.EnglishName);
			}
			else
			{
				char c = stringBuilder[0];
				char c2 = char.ToUpper(c, availableLocalization);
				if (c != c2)
				{
					stringBuilder.Replace(c, c2, 0, 1);
				}
			}
			yield return new DisplayLanguage
			{
				Label = stringBuilder.ToString(),
				Value = availableLocalization.Name
			};
		}
	}

	public DisplayOptions GetDisplayOptions(string userName)
	{
		ProfileData orCreateProfile = _profileRepository.GetOrCreateProfile(userName);
		return new DisplayOptions
		{
			Language = GetUserPreferredLanguage(orCreateProfile),
			TouchSupport = GetTouchSupport(orCreateProfile),
			HeadlessModeEnabled = GetHeadlessModeEnabled(orCreateProfile)
		};
	}

	private bool ValidateEmail(string email)
	{
		try
		{
			new MailAddress(email);
			return true;
		}
		catch (FormatException)
		{
			return false;
		}
	}

	private bool ValidatePassword(UserMembershipSaveInfo user)
	{
		if (!user.IsAdminMode && string.IsNullOrEmpty(user.CurrentPassword))
		{
			return false;
		}
		if (user.Password.Length.CompareTo(_securityConfiguration.UiUserProvider.MinRequiredPasswordLength) < 0)
		{
			return false;
		}
		if (user.Password.CompareTo(user.ConfirmPassword) != 0)
		{
			return false;
		}
		return true;
	}
}
