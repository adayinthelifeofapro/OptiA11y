namespace EPiServer.Shell.Web;

/// <summary>
///       A username and password validation function.
///       </summary>
/// <param name="userName">The user name.</param>
/// <param name="password">The password</param>
/// <returns>True if the user was sucessfully authenticated.</returns>
public delegate bool ValidateUserNamePassword(string userName, string password);
