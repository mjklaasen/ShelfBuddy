using System.Net.Http.Json;

namespace ShelfBuddy.ClientInterface;

public interface IUserService
{
    Task<UserInfo?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> LoginAsync(string username, string password);
    Task LogoutAsync();
}

public class UserInfo
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

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
            _currentUser = await client.GetFromJsonAsync<UserInfo>("/user/current");
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
        var response = await client.PostAsJsonAsync("/auth/login", loginRequest);

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

public class MockUserService : IUserService
{
    // Predefined mock users
    private readonly Dictionary<string, UserInfo> _mockUsers = new()
    {
        {
            "user1",
            new UserInfo
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "user1",
                Email = "user1@example.com"
            }
        },
        {
            "user2",
            new UserInfo
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Username = "user2",
                Email = "user2@example.com"
            }
        },
        {
            "admin",
            new UserInfo
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Username = "admin",
                Email = "admin@example.com"
            }
        }
    };

    // Store current user between sessions
    private UserInfo? _currentUser;

    // For persistence between app sessions (optional)
    private readonly IPreferences? _preferences;

    public MockUserService(IPreferences? preferences = null)
    {
        _preferences = preferences;
        if (_preferences is not null)
        {
            // Try to restore last logged in user
            var lastUsername = _preferences.Get("LastLoggedInUser", string.Empty);
            if (!string.IsNullOrEmpty(lastUsername) && _mockUsers.TryGetValue(lastUsername, out var user))
            {
                _currentUser = user;
            }
        }
    }

    public Task<UserInfo?> GetCurrentUserAsync()
    {
        // Simply return the current user from memory
        return Task.FromResult(_currentUser);
    }

    public Task<bool> IsAuthenticatedAsync()
    {
        // Check if we have a current user
        return Task.FromResult(_currentUser != null);
    }

    public Task<bool> LoginAsync(string username, string password)
    {
        // For the mock service, we'll accept any password for the predefined users
        if (_mockUsers.TryGetValue(username, out var user))
        {
            _currentUser = user;

            // Save the logged in user if preferences are available
            _preferences?.Set("LastLoggedInUser", username);

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task LogoutAsync()
    {
        _currentUser = null;

        // Clear the saved user
        _preferences?.Remove("LastLoggedInUser");

        return Task.CompletedTask;
    }
}