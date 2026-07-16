using InventoryManagement.Application.Interfaces.Repositories;

namespace InventoryManagement.Application.Interfaces.UnitOfWork;

public interface IUnitOfWork
{
    IProductRepository Products { get; }

    ICategoryRepository Categories { get; }

    Task<int> SaveChangesAsync();
}