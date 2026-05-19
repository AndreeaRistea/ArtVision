using VisionArt.Web.Client.IServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using VisionArt.Shared.DTOs;
using VisionArt.Web.Client.Helpers;

namespace VisionArt.Web.Client.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(
       HttpClient httpClient,
       ILocalStorageService localStorage,
       AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> Login(LoginRequestDto model)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "auth/login",
            model);

        if (!response.IsSuccessStatusCode)
            return false;

        var authResponse =
            await response.Content.ReadFromJsonAsync<AuthResponse>();

        await _localStorage.SetItemAsync("token", authResponse.Token);

        ((CustomAuthStateProvider)_authStateProvider)
            .NotifyUserAuthentication(authResponse.Token);

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "bearer",
                authResponse.Token);

        return true;
    }

    public async Task<string?> Register(RegisterRequestDto model)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "auth/register",
            model);

        if (response.IsSuccessStatusCode)
            return null;

        var body = await response.Content.ReadAsStringAsync();

        try
        {
            var errorObj = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(body);

            if (errorObj.TryGetProperty("errors", out var errors) && errors.GetArrayLength() > 0)
            {
                var first = errors[0];
                if (first.TryGetProperty("message", out var msg))
                    return msg.GetString();
            }

            if (errorObj.TryGetProperty("title", out var title))
                return title.GetString();
        }
        catch { }

        return "Registration failed. Please check your input and try again.";
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("token");

        ((CustomAuthStateProvider)_authStateProvider)
            .NotifyUserLogout();

        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}

