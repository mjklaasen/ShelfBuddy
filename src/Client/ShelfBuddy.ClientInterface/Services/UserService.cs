using System.Net.Http.Json;

namespace ShelfBuddy.ClientInterface.Services;

public class UserService(IHttpClientFactory httpClientFactory) : IUserService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private UserInfo? _currentUser;

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
        {
            return _currentUser;
        }

        var client = _httpClientFactory.CreateClient("api");
        try
        {
            _currentUser = await client.GetFromJsonAsync<UserInfo>("/api/v1/user/current");
            return _currentUser;
        }
        catch
        {
            // Failed to get user, likely not logged in
            return null;
        }
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return await GetCurrentUserAsync() != null;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var client = _httpClientFactory.CreateClient("api");

        var loginRequest = new { Username = username, Password = password };
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        if (response.IsSuccessStatusCode)
        {
            _currentUser = await response.Content.ReadFromJsonAsync<UserInfo>();
            return true;
        }

        return false;
    }

    public Task LogoutAsync()
    {
        _currentUser = null;
        return Task.CompletedTask;
    }
}