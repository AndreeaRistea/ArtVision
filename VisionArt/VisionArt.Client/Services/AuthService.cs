using VisionArt.Client.IServices;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using VisionArt.Client.DTOs;
using VisionArt.Client.Helpers;

namespace VisionArt.Client.Services;

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
            "api/auth/login",
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

    public async Task<bool> Register(RegisterRequestDto model)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/register",
            model);

        return response.IsSuccessStatusCode;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("token");

        ((CustomAuthStateProvider)_authStateProvider)
            .NotifyUserLogout();

        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}

