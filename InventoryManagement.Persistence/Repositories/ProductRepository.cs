using InventoryManagement.Application.DTOs.Products;
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

    public async Task<List<Product>> GetAllAsync(ProductQueryParameters parameters)
    {
        var query = _context.Products
            .Include(x => x.Category)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Keyword))
        {
            query = query.Where(x =>
                x.Name.Contains(parameters.Keyword) ||
                x.Code.Contains(parameters.Keyword));
        }

        query = query.OrderBy(x => x.Id);

        query = query.Skip((parameters.Page - 1) * parameters.PageSize)
                     .Take(parameters.PageSize);

        return await query.ToListAsync();
    }

}