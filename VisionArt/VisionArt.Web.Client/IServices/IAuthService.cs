using VisionArt.Shared.DTOs;

namespace VisionArt.Web.Client.IServices;

public interface IAuthService
{
    Task<bool> Login(LoginRequestDto model);
    Task<string?> Register(RegisterRequestDto model);
    Task Logout();
}

