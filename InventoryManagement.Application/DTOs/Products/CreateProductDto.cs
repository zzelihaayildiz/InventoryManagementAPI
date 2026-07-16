namespace InventoryManagement.Application.DTOs.Products;

public class CreateProductDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public int CategoryId { get; set; }
}