using AutoMapper;
using Avanti.Core.Microservice.Actors;
using Avanti.Core.Unittests;
using Avanti.ProductService.Product;
using Avanti.ProductService.Product.Api;
using Avanti.ProductService.Product.Mappings;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Avanti.ProductServiceTests.Product.Api;

public partial class PrivateApiControllerSpec : WithSubject<PrivateApiController>
{
    private readonly ProgrammableActor<ProductActor> progProductActor;

    private PrivateApiControllerSpec()
    {
        progProductActor = Kit.CreateProgrammableActor<ProductActor>("product-actor");
        IActorProvider<ProductActor> productActorProvider = An<IActorProvider<ProductActor>>();
        productActorProvider.Get().Returns(progProductActor.TestProbe);

        var config = new MapperConfiguration(cfg => cfg.AddProfile(new ProductMapping()));
        config.AssertConfigurationIsValid();

        Subject = new PrivateApiController(
            productActorProvider,
            An<ILogger<PrivateApiController>>(),
            config.CreateMapper());
    }
}
