using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;

    public CustomAuthStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Replace with your logic to get the token
        var token = await GetTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Headers.UserAgent.ParseAdd("request");

        Console.WriteLine("Requesting user info...");

        var response = await _httpClient.SendAsync(request);

        Console.WriteLine(response.StatusCode);

        if (response.IsSuccessStatusCode)
        {
            var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userInfo.Login),
                new Claim(ClaimTypes.NameIdentifier, userInfo.Id.ToString()),
                new Claim("urn:github:avatar", userInfo.AvatarUrl)
            }, "GitHub");

            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    private async Task<string> GetTokenAsync()
    {
        return await Task.FromResult<string>(null);
    }
}

public class UserInfo
{
    public string Login { get; set; }
    public int Id { get; set; }
    public string AvatarUrl { get; set; }
}