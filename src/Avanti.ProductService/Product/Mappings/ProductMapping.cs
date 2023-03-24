using AutoMapper;
using Avanti.ProductService.Product.Api;
using Avanti.ProductService.Product.Documents;
using Avanti.ProductService.Product.Events;

namespace Avanti.ProductService.Product.Mappings;

public class ProductMapping : Profile
{
    public ProductMapping()
    {
        CreateMap<PrivateApiController.PostProductRequest, ProductActor.UpsertProduct>();
        CreateMap<PrivateApiController.PostProductRequest.Property, ProductActor.UpsertProduct.Property>();
        CreateMap<ProductActor.ProductFound, PrivateApiController.GetProductResponse>()
            .AfterMap((src, dest, context) => context.Mapper.Map(src.Document, dest))
            .ForMember(s => s.Id, o => o.MapFrom(s => s.Id))
            .ForMember(s => s.Description, o => o.Ignore())
            .ForMember(s => s.Price, o => o.Ignore())
            .ForMember(s => s.WarehouseId, o => o.Ignore())
            .ForMember(s => s.Properties, o => o.Ignore());
        CreateMap<ProductDocument.Property, PrivateApiController.GetProductResponse.Property>();
        CreateMap<ProductDocument, PrivateApiController.GetProductResponse>()
            .ForMember(s => s.Id, o => o.Ignore());
        CreateMap<ProductActor.UpsertProduct, ProductDocument>();
        CreateMap<ProductActor.UpsertProduct.Property, ProductDocument.Property>();
        CreateMap<(int Id, ProductDocument Document), ProductUpdated>()
            .AfterMap((src, dest, context) => context.Mapper.Map(src.Document, dest))
            .ForMember(s => s.Id, o => o.MapFrom(s => s.Id));
        CreateMap<ProductDocument, ProductUpdated>()
            .ForMember(s => s.Id, o => o.Ignore());
        CreateMap<KeyValuePair<int, ProductDocument>, PrivateApiController.PostMultipleProductsResponse>()
            .ForMember(s => s.Id, o => o.MapFrom(s => s.Key))
            .ForMember(s => s.Description, o => o.MapFrom(s => s.Value.Description))
            .ForMember(s => s.Price, o => o.MapFrom(s => s.Value.Price))
            .ForMember(s => s.WarehouseId, o => o.MapFrom(s => s.Value.WarehouseId))
            .ForMember(s => s.Properties, o => o.MapFrom(s => s.Value.Properties));
        CreateMap<ProductDocument.Property, PrivateApiController.PostMultipleProductsResponse.Property>();
    }
}
