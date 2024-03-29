namespace SecuroTron.Lib.Services;

/// <summary>
/// Options for configuring <see cref="ActiveDirectoryService"/>.
/// </summary>
public sealed class ActiveDirectoryServiceOptions
{
    /// <summary>
    /// The fully qualified domain name of the Active Directory Domain Controller.
    /// </summary>
    public string ServerFqdn { get; set; } = null!;

    /// <summary>
    /// The username of the account to use for Active Directory operations.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// The password of the account to use for Active Directory operations.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// The domain name of the Active Directory domain.
    /// </summary>
    public string DomainName { get; set; } = null!;

    /// <summary>
    /// The root distinguished name of the Active Directory domain.
    /// </summary>
    public string RootDistiguishedName { get; set; } = null!;
}
