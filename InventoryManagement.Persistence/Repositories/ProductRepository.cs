using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Persistence.Context;
using InventoryManagement.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<Product?> GetByIdWithCategoryAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Product>> GetProductsWithCategoryAsync()
    {
        return await _context.Products
            .Include(x => x.Category)
            .ToListAsync();
    }

}