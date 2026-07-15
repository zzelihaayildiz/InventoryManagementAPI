namespace InventoryManagement.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }

    public DateTime CreatedDate { get; set; }

    public Category? Category { get; set; }
}