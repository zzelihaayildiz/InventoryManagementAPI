using InventoryManagement.Application.DTOs.Auth;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

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

        return new LoginResponseDto
        {
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(60),
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
}