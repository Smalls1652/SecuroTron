namespace SecuroTron.Lib.Services;

/// <summary>
/// Interface for services that interact with Active Directory.
/// </summary>
public interface IActiveDirectoryService
{
    void DisableUser(string username);
}
