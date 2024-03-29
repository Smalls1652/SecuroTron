using System.DirectoryServices.Protocols;

using SecuroTron.Lib.Extensions;

namespace SecuroTron.Lib.Models.ActiveDirectory;

/// <summary>
/// Represents a user account in Active Directory.
/// </summary>
public sealed class UserAccount
{
    private int _userAccountControlValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccount"/> class.
    /// </summary>
    /// <param name="entry">The <see cref="SearchResultEntry"/> to parse.</param>
    public UserAccount(SearchResultEntry entry)
    {
        ParseDirectoryAttributes(entry.Attributes);
    }

    /// <summary>
    /// The sAMAccountName of the user.
    /// </summary>
    public string? SamAccountName { get; set; }

    /// <summary>
    /// Indicates whether the account is enabled.
    /// </summary>
    public bool Enabled => !UserAccountControlFlags.Contains(UserAccountControlFlag.AccountDisabled);

    /// <summary>
    /// The distinguished name of the user.
    /// </summary>
    public string? DistinguishedName { get; set; }

    /// <summary>
    /// The user principal name of the user.
    /// </summary>
    public string? UserPrincipalName { get; set; }

    /// <summary>
    /// The raw user account control value.
    /// </summary>
    public int UserAccountControl => _userAccountControlValue;

    /// <summary>
    /// The individual user account control flags represented by <see cref="UserAccountControl"/>.
    /// </summary>
    public List<UserAccountControlFlag> UserAccountControlFlags => ParseUacValue();

    /// <summary>
    /// Parses the attributes of the directory entry.
    /// </summary>
    /// <param name="attributes">The <see cref="SearchResultAttributeCollection"/> to parse.</param>
    private void ParseDirectoryAttributes(SearchResultAttributeCollection attributes)
    {
        foreach (DirectoryAttribute attribute in attributes.Values)
        {
            switch (attribute.Name)
            {
                case "sAMAccountName":
                    SamAccountName = attribute.GetFirstValue<string>();
                    break;

                case "distinguishedName":
                    DistinguishedName = attribute.GetFirstValue<string>();
                    break;

                case "userPrincipalName":
                    UserPrincipalName = attribute.GetFirstValue<string>();
                    break;

                case "userAccountControl":
                    _userAccountControlValue = attribute.GetFirstValue<int>();
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Adds a user account control flag.
    /// </summary>
    /// <param name="flag">The <see cref="UserAccountControlFlag"/> to add.</param>
    public void AddUserAccountControlFlag(UserAccountControlFlag flag)
    {
        _userAccountControlValue |= (int)flag;
    }

    /// <summary>
    /// Removes a user account control flag.
    /// </summary>
    /// <param name="flag">The <see cref="UserAccountControlFlag"/> to remove.</param>
    public void RemoveUserAccountControlFlag(UserAccountControlFlag flag)
    {
        _userAccountControlValue &= ~(int)flag;
    }

    /// <summary>
    /// Parses the user account control value into individual flags.
    /// </summary>
    /// <returns>A collection of <see cref="UserAccountControlFlag"/> for the user account.</returns>
    private List<UserAccountControlFlag> ParseUacValue()
    {
        List<UserAccountControlFlag> userAccountControlFlags = [];

        foreach (var flag in Enum.GetValues<UserAccountControlFlag>())
        {
            if ((_userAccountControlValue & (int)flag) == (int)flag)
            {
                userAccountControlFlags.Add(flag);
            }
        }

        return userAccountControlFlags;
    }
}
