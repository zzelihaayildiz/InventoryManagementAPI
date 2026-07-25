using AutoMapper;
using FluentValidation;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Services.Implementations;
using InventoryManagement.Domain.Entities;
using Moq;
using FluentValidation.Results;

namespace InventoryManagement.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateProductDto>> _validatorMock;

    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();

        _mapperMock = new Mock<IMapper>();

        _validatorMock = new Mock<IValidator<CreateProductDto>>();


        _productService = new ProductService(
            _productRepositoryMock.Object,
            _mapperMock.Object,
             _validatorMock.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Product()
    {
        var dto = new CreateProductDto
        {
            Name = "Laptop",
            Code = "P001",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        var product = new Product
        {
            Id = 1,
            Name = dto.Name,
            Code = dto.Code,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        _validatorMock
    .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
    .ReturnsAsync(new ValidationResult());

        _mapperMock
    .Setup(x => x.Map<Product>(dto))
    .Returns(product);

        var id = await _productService.CreateAsync(dto);

        _productRepositoryMock.Verify(
    x => x.AddAsync(product),
    Times.Once);

        _productRepositoryMock.Verify(
    x => x.SaveChangesAsync(),
    Times.Once);

        Assert.Equal(1, id);
    }
}