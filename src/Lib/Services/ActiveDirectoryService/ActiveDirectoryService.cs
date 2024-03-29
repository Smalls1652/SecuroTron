using System.DirectoryServices.Protocols;
using System.Net;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SecuroTron.Lib.Models.ActiveDirectory;

namespace SecuroTron.Lib.Services;

/// <summary>
/// Service for interacting with Active Directory.
/// </summary>
public sealed class ActiveDirectoryService : IActiveDirectoryService, IDisposable
{
    private bool _disposed;

    private readonly ActiveDirectoryServiceOptions _options;
    private readonly ILogger _logger;
    private readonly LdapConnection _ldapConnection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveDirectoryService"/> class.
    /// </summary>
    /// <param name="options">The <see cref="ActiveDirectoryServiceOptions"/>.</param>
    /// <param name="logger">The <see cref="ILogger"/>.</param>
    public ActiveDirectoryService(IOptions<ActiveDirectoryServiceOptions> options, ILogger<ActiveDirectoryService> logger)
    {
        _options = options.Value;
        _logger = logger;

        _ldapConnection = new(
            identifier: new(
                server: _options.ServerFqdn,
                portNumber: 636
            )
        )
        {
            AuthType = AuthType.Basic
        };

        _ldapConnection.SessionOptions.ReferralChasing = ReferralChasingOptions.None;
        _ldapConnection.SessionOptions.ProtocolVersion = 2;
        _ldapConnection.SessionOptions.SecureSocketLayer = true;

        NetworkCredential credential = new(
            userName: _options.Username,
            password: _options.Password
        );

        try
        {
            _ldapConnection.Bind(credential);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to bind to the LDAP server.");

            throw;
        }

        //_distinguishedName = GetRootDn();
    }

    /// <summary>
    /// Disable a user account.
    /// </summary>
    /// <param name="username">The username (sAMAccountName) of the user to disable.</param>
    public void DisableUser(string username)
    {
        SearchRequest searchRequest = new(
            distinguishedName: _options.RootDistiguishedName,
            ldapFilter: $"(&(objectClass=user)(sAMAccountName={username}))",
            searchScope: SearchScope.Subtree
        );

        SearchResponse searchResponse = (SearchResponse)_ldapConnection.SendRequest(searchRequest);

        if (searchResponse.Entries.Count == 0)
        {
            _logger.LogWarning("User {Username} not found.", username);
            return;
        }

        foreach (SearchResultEntry entry in searchResponse.Entries)
        {
            UserAccount userAccount = new(entry);

            if (!userAccount.Enabled)
            {
                _logger.LogWarning("[{Username} ({UserPrincipalName})] Already disabled.", userAccount.SamAccountName, userAccount.UserPrincipalName);
                return;
            }

            userAccount.AddUserAccountControlFlag(UserAccountControlFlag.AccountDisabled);

            ModifyRequest modifyRequest = new(
                distinguishedName: entry.DistinguishedName,
                operation: DirectoryAttributeOperation.Replace,
                attributeName: "userAccountControl",
                values: [
                    userAccount.UserAccountControl.ToString()
                ]
            );

            _ldapConnection.SendRequest(modifyRequest);

            _logger.LogInformation("[{Username} ({UserPrincipalName})] Successfully disabled", userAccount.SamAccountName, userAccount.UserPrincipalName);

        }
    }

    private string GetRootDn()
    {
        SearchRequest searchRequest = new(
            distinguishedName: null,
            ldapFilter: "(objectClass=*)",
            searchScope: SearchScope.Base
        );

        var searchResponse = (SearchResponse)_ldapConnection.SendRequest(searchRequest);

        return searchResponse.Entries[0].DistinguishedName;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        _ldapConnection.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
