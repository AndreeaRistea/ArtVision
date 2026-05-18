using VisionArt.Client.DTOs;

namespace VisionArt.Client.IServices;

public interface IAuthService
{
    Task<bool> Login(LoginRequestDto model);
    Task<bool> Register(RegisterRequestDto model);
    Task Logout();
}

