using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace VisionArt.Client.Helpers;

public class CustomAuthStateProvider : AuthenticationStateProvider

{
    private readonly ILocalStorageService _localStorage;

    public CustomAuthStateProvider(
        ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("token");

        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = ParseClaimsFromJwt(token);

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "jwt"));

        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var claims = ParseClaimsFromJwt(token);

        var authenticatedUser = new ClaimsPrincipal(
            new ClaimsIdentity(claims, "jwt"));

        var authState = Task.FromResult(
            new AuthenticationState(authenticatedUser));

        NotifyAuthenticationStateChanged(authState);
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(
            new ClaimsIdentity());

        var authState = Task.FromResult(
            new AuthenticationState(anonymous));

        NotifyAuthenticationStateChanged(authState);
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();

        var token = handler.ReadJwtToken(jwt);

        return token.Claims;
    }
}

