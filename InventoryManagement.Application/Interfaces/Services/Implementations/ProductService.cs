using AutoMapper;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Application.Interfaces.Repositories;
using InventoryManagement.Application.Interfaces.Services;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task CreateAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);

        product.CreatedDate = DateTime.Now;

        await _productRepository.AddAsync(product);

        await _productRepository.SaveChangesAsync();
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetProductsWithCategoryAsync();

        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product =await _productRepository.GetByIdAsync(id);

        if (product is null)
            return null;

        return _mapper.Map<ProductDto>(product);
    }
}