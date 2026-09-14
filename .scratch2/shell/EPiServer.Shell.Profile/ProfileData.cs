using System.Collections.Generic;
using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace EPiServer.Shell.Profile;

/// <summary>
///       Contains profile information for a user.
///       </summary>
[EPiServerDataStore(AutomaticallyRemapStore = true)]
[EPiServerDataTable(TableName = "tblSystemBigTable")]
public class ProfileData : IDynamicData
{
	/// <summary>
	///       Gets or sets the stored item identity.
	///       </summary>
	public Identity Id { get; set; }

	/// <summary>
	///       Gets or sets the user name of the user.
	///       </summary>
	public string UserName { get; set; }

	/// <summary>
	///       Gets a dictionary of settings associated with the user.
	///       </summary>
	public Dictionary<string, object> Settings { get; private set; }

	/// <summary>
	///       Creats an instance of the <see cref="T:EPiServer.Shell.Profile.ProfileData" /> class.
	///       </summary>
	public ProfileData()
	{
		Settings = new Dictionary<string, object>();
	}
}
