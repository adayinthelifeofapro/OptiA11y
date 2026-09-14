using System;
using System.Linq;
using EPiServer.Data.Dynamic;
using EPiServer.Framework.Cache;
using EPiServer.Shell.Composition;

namespace EPiServer.Shell.Profile;

/// <summary>
///       Provides access to a user profile.
///       </summary>
public class ProfileRepository : IProfileRepository
{
	private readonly DynamicDataStoreFactory _storeFactory;

	private readonly ISynchronizedObjectInstanceCache _cache;

	private const string CacheKey = "ProfileRepositoryCacheKey:";

	private DynamicDataStore Store => _storeFactory.GetOrCreateStore(typeof(ProfileData));

	/// <inheritdoc />
	public event EventHandler<ProfileEventArgs> ProfileCreated;

	/// <summary>
	///       Creates a new instance of the <see cref="T:EPiServer.Shell.Profile.ProfileRepository" /> class.
	///       </summary>
	/// <param name="storeFactory">The store factory to use for creating stores.</param>
	/// <param name="cache">
	/// </param>
	public ProfileRepository(DynamicDataStoreFactory storeFactory, ISynchronizedObjectInstanceCache cache)
	{
		_storeFactory = storeFactory;
		_cache = cache;
	}

	/// <inheritdoc />
	public ProfileData GetProfile(string userName)
	{
		ArgumentNullException.ThrowIfNull(userName, "userName");
		string text = "ProfileRepositoryCacheKey:" + userName;
		if (((IObjectInstanceCache)_cache).Get(text) is ProfileData result)
		{
			return result;
		}
		ProfileData profileData = Store.Find<ProfileData>("UserName", (object)userName).FirstOrDefault();
		if (profileData == null)
		{
			return null;
		}
		((IObjectInstanceCache)_cache).Insert(text, (object)profileData, CacheEvictionPolicy.Empty);
		return profileData;
	}

	/// <inheritdoc />
	public ProfileData GetOrCreateProfile(string userName)
	{
		ArgumentNullException.ThrowIfNull(userName, "userName");
		return GetProfile(userName) ?? Create(userName);
	}

	/// <inheritdoc />
	public void Save(ProfileData profile)
	{
		ArgumentNullException.ThrowIfNull(profile, "profile");
		RemoveFromCache(profile.UserName);
		Store.Save((object)profile);
	}

	/// <inheritdoc />
	public void Delete(string userName)
	{
		ArgumentNullException.ThrowIfNull(userName, "userName");
		ProfileData profile = GetProfile(userName);
		if (profile != null)
		{
			RemoveFromCache(userName);
			Store.Delete((object)profile);
		}
	}

	private void RemoveFromCache(string userName)
	{
		string text = "ProfileRepositoryCacheKey:" + userName;
		((IObjectInstanceCache)_cache).Remove(text);
	}

	private ProfileData Create(string userName)
	{
		ProfileData profileData = new ProfileData
		{
			UserName = userName
		};
		ProfileCreated?.Invoke(this, new ProfileEventArgs
		{
			Profile = profileData
		});
		return profileData;
	}
}
