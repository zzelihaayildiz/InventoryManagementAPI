using InventoryManagement.Application.DTOs.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace InventoryManagement.Tests.Integration;

public class ProductApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_Should_ReturnSuccessStatusCode()
    {
        var response = await _client.GetAsync("/api/Products");

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_ReturnCreated()
    {
        //Arrange

        var dto = new CreateProductDto
        {
            Name = "Integration Laptop",
            Code = "INT001",
            Price = 500,
            Stock = 10,
            CategoryId = 1
        };

        //Act
        var response = await _client.PostAsJsonAsync("/api/Products", dto);

        //Assert 

        Assert.Equal(HttpStatusCode.Created,
            response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_Should_ReturnBadRequest_When_ProductIsInvalid()
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

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/Products",
            dto);

        // Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }

    [Fact]
    public async Task GetProduct_Should_ReturnNotFound_When_ProductDoesNotExist()
    {
        //Arrange
        var productId = 99999;

        //Act

        var response = await _client.GetAsync($"api/Products/{productId}");

        //Assert

        Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
    }

    [Fact]
    public async Task UpdateProduct_Should_ReturnNoContent_When_ProductExists()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Update Test Product",
            Code = "UPD001",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Products",
            createDto);

        var result = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();

        var productId = result!.Data;

        var updateDto = new UpdateProductDto
        {
            Id = productId,
            Name = "Updated Product",
            Code = "UPD001",
            Price = 200,
            Stock = 20,
            CategoryId = 1
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/Products",
            updateDto);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_ReturnOK_When_ProductExists()
    {
        //Arrange

        var createDto = new CreateProductDto
        {
            Name = "Delete Test Product",
            Code = "DEL001",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };


        var createResponse = await _client.PostAsJsonAsync(
            "api/Products",
            createDto);

        var result = await createResponse.Content.ReadFromJsonAsync<ApiResponse<int>>();

        var productId = result!.Data;

        //Act

        var response = await _client.DeleteAsync($"/api/Products/{productId}");

        //Assert

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_RemoveProduct_When_ProductExists()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Delete Verification Product",
            Code = "DEL002",
            Price = 100,
            Stock = 10,
            CategoryId = 1
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/api/Products",
            createDto);

        var result =
            await createResponse.Content
                .ReadFromJsonAsync<ApiResponse<int>>();

        var productId = result!.Data;

        // Act
        var deleteResponse = await _client.DeleteAsync(
            $"/api/Products/{productId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync(
            $"/api/Products/{productId}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_Should_ReturnNotFound_When_ProductDoesNotExist()
    {
        // Arrange
        var productId = 99999;

        // Act
        var response = await _client.DeleteAsync(
            $"/api/Products/{productId}");

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}