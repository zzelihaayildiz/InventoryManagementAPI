using FluentValidation;
using InventoryManagement.Application.DTOs.Products;

namespace InventoryManagement.Application.Validators.Products;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Ürün kodu zorunludur.")
            .MaximumLength(20);

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Ürün adı zorunludur.")
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok negatif olamaz.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Kategori seçilmelidir.");
    }
}