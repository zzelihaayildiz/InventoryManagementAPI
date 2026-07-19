using InventoryManagement.Application.DTOs.Products;

namespace InventoryManagement.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync(ProductQueryParameters parameters);

    Task<ProductDto?> GetByIdAsync(int id);

    Task<int> CreateAsync(CreateProductDto dto);
    Task UpdateAsync(UpdateProductDto dto);
    Task DeleteAsync(int id);

}