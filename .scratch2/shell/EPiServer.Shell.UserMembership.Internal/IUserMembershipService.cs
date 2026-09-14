using System.Collections.Generic;
using System.Threading.Tasks;
using EPiServer.Shell.Security;

namespace EPiServer.Shell.UserMembership.Internal;

public interface IUserMembershipService
{
	Task<UserMembershipInfo> GetUser(string userName);

	Task<UserSaveResult> SaveUser(UserMembershipSaveInfo model);

	void SaveDisplayOptions(string userName, DisplayOptions model);

	string GetMessageFromStatus(UIUserCreateStatus status, string userName, IEnumerable<string> errors);

	Task<bool> ResetViews(string userName);

	Task<IEnumerable<string>> GetAllRoles();

	IEnumerable<DisplayLanguage> GetAvailableLanguages();

	DisplayOptions GetDisplayOptions(string userName);

	Task<UserSaveResult> CreateUser(UserMembershipSaveInfo model);
}
