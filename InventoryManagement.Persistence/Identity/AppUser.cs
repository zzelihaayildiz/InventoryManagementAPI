using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Persistence.Identity;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpireDate { get; set; }
}