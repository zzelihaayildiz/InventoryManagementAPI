using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryParameters parameters)
    {
        var products = await _productService.GetAllAsync(parameters);

        return Ok(
            ApiResponse<List<ProductDto>>
                .SuccessResponse(products, "Ürünler listelendi."));
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound(
                 ApiResponse<string>.FailResponse("Ürün bulunamadı."));

        return Ok(
            ApiResponse<ProductDto>
                .SuccessResponse(product, "Ürün getirildi."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var id = await _productService.CreateAsync(dto);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id },
            ApiResponse<int>.SuccessResponse(id, "Ürün başarıyla oluşturuldu."));
    }

    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> Update(UpdateProductDto dto)
    {
        await _productService.UpdateAsync(dto);

        return Ok(ApiResponse<string>.SuccessResponse(
            string.Empty,
            "Ürün başarıyla kaydedildi."));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);

        return Ok(ApiResponse<string>.SuccessResponse(
            string.Empty, 
            "Ürün başarıyla silindi."));
    }

}
