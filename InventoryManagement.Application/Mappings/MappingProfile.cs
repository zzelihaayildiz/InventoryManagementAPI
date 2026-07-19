using AutoMapper;
using InventoryManagement.Application.DTOs.Products;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null
                    ? src.Category.Name
                    : string.Empty));

        CreateMap<CreateProductDto, Product>();

        CreateMap<UpdateProductDto, Product>();

    }
}