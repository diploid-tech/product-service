using AutoMapper;
using Avanti.Core.Microservice.Actors;
using Microsoft.AspNetCore.Mvc;

namespace Avanti.ProductService.Product.Api;

[Route("/private/product")]
[ApiController]
public partial class PrivateApiController
{
    private readonly IMapper mapper;
    private readonly IActorRef productActorRef;
    private readonly ILogger logger;

    public PrivateApiController(
        IActorProvider<ProductActor> productActorProvider,
        ILogger<PrivateApiController> logger,
        IMapper mapper)
    {
        this.productActorRef = productActorProvider.Get();
        this.logger = logger;
        this.mapper = mapper;
    }
}
