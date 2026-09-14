using System.Threading.Tasks;
using EPiServer.ServiceLocation;

namespace EPiServer.Shell.Security.Internal;

public class SecurityConfiguration
{
	public UIRoleProvider UiRoleProvider { get; set; }

	public UIUserProvider UiUserProvider { get; set; }

	public UIUserManager UiUserManager { get; set; }

	public UISignInManager UiSignInManager { get; set; }

	public bool IsRoleManagementEnabled => UiRoleProvider != null;

	public bool IsUserManagementEnabled => UiUserProvider != null;

	public async Task<IUIUser> GetUserAsync(string userName)
	{
		if (IsUserManagementEnabled)
		{
			return await UiUserProvider.GetUserAsync(userName);
		}
		return new UiUser
		{
			Username = userName
		};
	}

	private static T TryGetInstance<T>()
	{
		object obj = default(object);
		if (ServiceProviderExtensions.TryGetExistingInstance(ServiceLocator.Current, typeof(T), ref obj))
		{
			return (T)obj;
		}
		return default(T);
	}

	public SecurityConfiguration()
	{
		UiUserProvider = TryGetInstance<UIUserProvider>();
		UiRoleProvider = TryGetInstance<UIRoleProvider>();
		UiUserManager = TryGetInstance<UIUserManager>();
		UiSignInManager = TryGetInstance<UISignInManager>();
	}
}
