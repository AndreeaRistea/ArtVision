using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using VisionArtAPI.DTOs;

namespace VisionArtAPI.IServices;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequestDto request);

    Task<IdentityResult> RegisterAsync(RegisterRequestDto request);
}

