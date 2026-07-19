using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Interfaces.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByIdWithCategoryAsync(int id);

    Task<List<Product>> GetAllAsync(ProductQueryParameters parameters);
}