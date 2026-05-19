using Microsoft.AspNetCore.Identity;
using VisionArt.Shared.DTOs;
using VisionArt.Api.Helpers;
using VisionArt.Api.IServices;
using VisionArt.Api.Models;

namespace VisionArt.Api.Services;

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
            Token = token,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public async Task<IdentityResult> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "An account with this email already exists"
            });
        }

        var user = new User
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };

        return await _userManager.CreateAsync(user, request.Password);
    }
}
