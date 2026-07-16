using InventoryManagement.Application.DTOs.Products;

namespace InventoryManagement.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();

    Task<ProductDto?> GetByIdAsync(int id);

    Task CreateAsync(CreateProductDto dto);
}