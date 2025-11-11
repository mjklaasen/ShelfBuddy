namespace ShelfBuddy.ClientInterface.Services;

internal interface IUserService
{
    Task<UserInfo?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
}
