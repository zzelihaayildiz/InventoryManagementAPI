using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using InventoryManagement.Application.Common.Exceptions;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper,
        IValidator<CreateProductDto> createValidator,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _createValidator = createValidator;
        _logger = logger;
    }

    public async Task<int> CreateAsync(CreateProductDto dto)
    {
        var validationResult = await _createValidator.ValidateAsync(dto);

        throw new Exception("Logging test exception");
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(
                "Product creation validation failed. Code: {Code}",
                dto.Code);

            throw new ValidationException(validationResult.Errors);
        }

        var product = _mapper.Map<Product>(dto);

        product.CreatedDate = DateTime.UtcNow;

        await _productRepository.AddAsync(product);

        await _productRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Product created successfully. ProductId: {ProductId}, Code: {Code}",
            product.Id,
            product.Code);

        return product.Id;
    }

    public async Task<List<ProductDto>> GetAllAsync(ProductQueryParameters parameters)
    {
        var products = await _productRepository.GetAllAsync(parameters);

        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdWithCategoryAsync(id);
        if (product is null)
            return null;

        return _mapper.Map<ProductDto>(product);
    }

    public async Task UpdateAsync(UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.Id);

        if (product is null)
            throw new NotFoundException("Ürün bulunamadı.");

        _mapper.Map(dto, product);

        _productRepository.Update(product);

        await _productRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
            throw new NotFoundException("Ürün Bulunamadı.");

        _productRepository.Delete(product);

        await _productRepository.SaveChangesAsync();
    }
}