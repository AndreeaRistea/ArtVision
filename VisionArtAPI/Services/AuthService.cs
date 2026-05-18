using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using VisionArtAPI.DTOs;
using VisionArtAPI.Helpers;
using VisionArtAPI.IServices;
using VisionArtAPI.Models;

namespace VisionArtAPI.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenGenerator _jwtGenerator;

    public AuthService(
        UserManager<User> userManager,
        JwtTokenGenerator jwtGenerator)
    {
        _userManager = userManager;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
            return null;

        var validPassword =
            await _userManager.CheckPasswordAsync(user, request.Password);

        if (!validPassword)
            return null;

        var token = _jwtGenerator.Generate(user);

        return new AuthResponse
        {
            Token = token
        };
    }

    public async Task<IdentityResult> RegisterAsync(RegisterRequestDto request)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };

        return await _userManager.CreateAsync(user, request.Password);
    }
}

