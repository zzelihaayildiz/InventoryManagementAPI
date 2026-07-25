using InventoryManagement.Application.DTOs.Auth;

namespace InventoryManagement.Application.Interfaces.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto);
}
