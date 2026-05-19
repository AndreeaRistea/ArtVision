using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using VisionArt.Shared.DTOs;

namespace VisionArt.Api.IServices;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequestDto request);

    Task<IdentityResult> RegisterAsync(RegisterRequestDto request);
}

