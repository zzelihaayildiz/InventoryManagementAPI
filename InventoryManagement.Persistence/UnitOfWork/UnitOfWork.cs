using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.UnitOfWork;
using InventoryManagement.Persistence.Context;

namespace InventoryManagement.Persistence.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IProductRepository Products { get; }

    public ICategoryRepository Categories { get; }

    public UnitOfWork(
        AppDbContext context,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _context = context;

        Products = productRepository;
        Categories = categoryRepository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}