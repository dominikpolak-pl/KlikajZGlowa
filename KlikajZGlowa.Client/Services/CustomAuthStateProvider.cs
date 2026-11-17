using KlikajZGlowa.Client.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace KlikajZGlowa.Client.Services;

public sealed class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("accessToken");

        return new AuthenticationState(CreatePrincipal(token));
    }

    public void NotifyUserLogin(string token)
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(CreatePrincipal(token))));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(CreatePrincipal(null))));
    }

    private static ClaimsPrincipal CreatePrincipal(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }
        var claims = ParseClaimsFromJwt(token);

        var identity = new ClaimsIdentity(claims, "Bearer", ClaimTypes.Name, ClaimTypes.Role);

        return new ClaimsPrincipal(identity);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var parts = jwt.Split('.');
        if (parts.Length < 2)
        {
            return claims;
        }

        var payload = parts[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null)
        {
            return claims;
        }

        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);
        if (roles == null)
        {
            keyValuePairs.TryGetValue("role", out roles);
        }

        if (roles != null)
        {
            var rolesString = roles.ToString()!.Trim();

            if (rolesString.StartsWith("["))
            {
                var parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);
                if (parsedRoles != null)
                {
                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, rolesString));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
            keyValuePairs.Remove("role");
        }

        claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}