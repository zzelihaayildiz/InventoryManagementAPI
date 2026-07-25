using InventoryManagement.Application.DTOs.Auth;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<AppUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user =await _userManager.FindByEmailAsync(dto.Email);

        if (user == null)
            throw new Exception("Email veya şifre hatalı.");

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);

        if (!isPasswordCorrect)
            throw new Exception("Email veya şifre hatalı.");

        var roles = await _userManager.GetRolesAsync(user);

        var token = _tokenService.CreateToken(
         new TokenUserDto
         {
             Id = user.Id,
             UserName = user.UserName!,
             Email = user.Email!,
             Roles = roles
         });

        var refreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return new LoginResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            RefreshTokenExpireDate = user.RefreshTokenExpireDate.Value,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };
    }

    public async Task RegisterAsync(RegisterDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.UserName,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new Exception(errors);
        }

        await _userManager.AddToRoleAsync(user, "Customer");
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        var user =await _userManager.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == dto.RefreshToken);

        if (user is null)
            throw new Exception("Geçersiz Refresh Token.");

        if (user.RefreshTokenExpireDate <= DateTime.UtcNow)
            throw new Exception("Refresh Token süresi dolmuş.");

        var roles = await _userManager.GetRolesAsync(user);

        var token = _tokenService.CreateToken(
            new TokenUserDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Roles = roles
            });

        var newRefreshToken = _tokenService.CreateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpireDate = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return new LoginResponseDto {Token = token,
            RefreshToken = newRefreshToken,
            Expiration = DateTime.UtcNow.AddMinutes(60),
            RefreshTokenExpireDate = user.RefreshTokenExpireDate.Value,
            UserName = user.UserName!,
            Email = user.Email!,
            Roles = roles
        };

    }
}