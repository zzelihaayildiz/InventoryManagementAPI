using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Services.Implementations;
using InventoryManagement.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Runtime.InteropServices;

namespace InventoryManagement.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IValidator<CreateProductDto>> _validatorMock;

    private readonly ProductService _productService;
    private readonly Mock<ILogger<ProductService>> _loggerMock;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _validatorMock = new Mock<IValidator<CreateProductDto>>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _productService = new ProductService(
            _productRepositoryMock.Object,
            _mapperMock.Object,
             _validatorMock.Object,
             _loggerMock.Object);
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

    [Fact]
    public async Task CreateAsync_Should_ThrowValidationException_When_ProductIsInvalid()
    {
        // Arrange
        var dto = new CreateProductDto
        {
            Name = "",
            Code = "",
            Price = -10,
            Stock = -1,
            CategoryId = 0
        };

        _validatorMock
            .Setup(x => x.ValidateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult(
                new[]
                {
                new FluentValidation.Results.ValidationFailure(
                    "Name",
                    "Ürün adı boş olamaz.")
                }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _productService.CreateAsync(dto));

        _productRepositoryMock.Verify(
    x => x.AddAsync(It.IsAny<Product>()),
    Times.Never);

        _productRepositoryMock.Verify(
    x => x.SaveChangesAsync(),
    Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Product_When_ProductExists()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Code = "P001",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        var productDto = new ProductDto
        {
            Id = 1,
            Name = "Laptop",
            Code = "P001",
            Price = 100,
            Stock = 10,
            CategoryName = "Elektronik"
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdWithCategoryAsync(1))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(x => x.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _productService.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);

        _productRepositoryMock.Verify(
            x => x.GetByIdWithCategoryAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_When_ProductDoesNotExist()
    {
        _productRepositoryMock
            .Setup(x => x.GetByIdWithCategoryAsync(1))
            .ReturnsAsync((Product?)null);

        var result = await _productService.GetByIdAsync(1);

        Assert.Null(result);

        _productRepositoryMock.Verify(
            x => x.GetByIdWithCategoryAsync(1),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Product_When_ProductExists()
    {

        //Arrange

        var dto = new UpdateProductDto
        {
            Id = 1,
            Name = "Updated Laptop",
            Code = "P001",
            Price = 150,
            Stock = 20,
            CategoryId = 1
        };

        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Code = "P001",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        //Act

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        await _productService.UpdateAsync(dto);


        //Assert/Verify
       
        _mapperMock.Verify(
            x => x.Map(dto,product),
            Times.Once);

        _productRepositoryMock.Verify(
            x => x.Update(product),
            Times.Once);

        _productRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);
    }
    [Fact]
    public async Task UpdateAsync_Should_ThrowNotFoundException_When_ProductDoesNotExist()
    {
        // Arrange
        var dto = new UpdateProductDto
        {
            Id = 999,
            Name = "Updated Laptop",
            Code = "P999",
            Price = 150,
            Stock = 20,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act & Assert

        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.UpdateAsync(dto));

        _productRepositoryMock.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Never);

        _productRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);

    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Product_When_ProductExists()
    {
        //Arrange

        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Code = "P001",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(product);

        //Act

        await _productService.DeleteAsync(1);

        //Assert

        _productRepositoryMock.Verify(
            x => x.GetByIdAsync(1),
            Times.Once);

        _productRepositoryMock.Verify(
            x => x.Delete(product),
            Times.Once);

        _productRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Once);

    }

    [Fact]
    public async Task DeleteAsync_Should_ThrowNotFoundException_When_ProductDoesNotExist()
    {
        //Arrange

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        //Act & Assert

        await Assert.ThrowsAsync<NotFoundException>(
            () => _productService.DeleteAsync(999));

        _productRepositoryMock.Verify(
            x => x.Delete(It.IsAny<Product>()),
            Times.Never);

        _productRepositoryMock.Verify(
            x => x.SaveChangesAsync(),
            Times.Never);
    }

}