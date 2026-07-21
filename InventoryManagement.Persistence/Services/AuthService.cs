using InventoryManagement.Application.DTOs.Auth;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Persistence.Identity;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Persistence.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;

    public AuthService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
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