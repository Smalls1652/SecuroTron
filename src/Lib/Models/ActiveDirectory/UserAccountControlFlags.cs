namespace SecuroTron.Lib.Models.ActiveDirectory;

/// <summary>
/// Flags that represent the user account control settings for an Active Directory user.
/// </summary>
public enum UserAccountControlFlag : int
{
    /// <summary>
    /// The logon script will be run.
    /// </summary>
    Script = 0x0001,

    /// <summary>
    /// The user account is disabled.
    /// </summary>
    AccountDisabled = 0x0002,

    /// <summary>
    /// The home directory is required.
    /// </summary>
    HomeDirectoryRequired = 0x0008,

    /// <summary>
    /// The account is currently locked out.
    /// </summary>
    AccountLockedOut = 0x0010,

    /// <summary>
    /// No password is required.
    /// </summary>
    PasswordNotRequired = 0x0020,

    /// <summary>
    /// The user can't change the password.
    /// </summary>
    PasswordCannotChange = 0x0040,

    /// <summary>
    /// The user can send an encrypted password.
    /// </summary>
    EncryptedTextPasswordAllowed = 0x0080,

    /// <summary>
    /// An account for users whose primary account is in another domain.
    /// </summary>
    TempDuplicateAccount = 0x0100,

    /// <summary>
    /// Default account type that represents a typical user.
    /// </summary>
    NormalAccount = 0x0200,

    /// <summary>
    /// Permit to trust an account for a system domain that trusts other domains.
    /// </summary>
    InterDomainTrustAccount = 0x0800,

    /// <summary>
    /// Computer account for a computer that is a member of this domain.
    /// </summary>
    WorkstationTrustAccount = 0x1000,

    /// <summary>
    /// A computer account for a domain controller that is a member of this domain.
    /// </summary>
    ServerTrustAccount = 0x2000,

    /// <summary>
    /// The password should never expire on the account.
    /// </summary>
    PasswordDoesNotExpire = 0x10000,

    /// <summary>
    /// A MNS logon account.
    /// </summary>
    MnsLogonAccount = 0x20000,

    /// <summary>
    /// Smart card is required for logins.
    /// </summary>
    SmartCardRequired = 0x40000,

    /// <summary>
    /// The account is trusted for Kerberos delegation.
    /// </summary>
    TrustedForDelegation = 0x80000,

    /// <summary>
    /// The security context of the user will not be delegated.
    /// </summary>
    AccountNotDelegated = 0x100000,

    /// <summary>
    /// Restrict this principal to use only Data Encryption Standard (DES) encryption types for keys.
    /// </summary>
    UseDesKeyOnly = 0x200000,

    /// <summary>
    /// Kerberos pre-authentication is not required for logon.
    /// </summary>
    DontRequirePreauth = 0x400000,

    /// <summary>
    /// The user's password has expired.
    /// </summary>
    PasswordExpired = 0x800000,

    /// <summary>
    /// Enabled for delegation.
    /// </summary>
    TrustedToAuthenticateForDelegation = 0x1000000,

    /// <summary>
    /// The account is a read-only domain controller (RODC).
    /// </summary>
    PartialSecretsAccount = 0x4000000,
}
