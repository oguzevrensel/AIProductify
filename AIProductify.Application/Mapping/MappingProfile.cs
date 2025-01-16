using AIProductify.Application.DTO.Attribute;
using AIProductify.Application.DTO.Product;
using AIProductify.Core.Entities;
using AutoMapper;

namespace AIProductify.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<ProductAttribute, ProductAttributeDto>().ReverseMap();
        }
    }
}
