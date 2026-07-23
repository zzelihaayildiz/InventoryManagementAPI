using InventoryManagement.Application.DTOs.Auth;

public interface ITokenService
{
    string CreateToken(TokenUserDto user);
}