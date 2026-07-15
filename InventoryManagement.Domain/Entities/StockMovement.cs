using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Domain.Entities;

public class StockMovement
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public MovementType MovementType { get; set; }

    public DateTime CreatedDate { get; set; }

    public Product? Product { get; set; }
}