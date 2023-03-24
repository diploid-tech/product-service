using Avanti.Core.Microservice.Actors;
using Avanti.Core.Microservice.Extensions;

namespace Avanti.ProductService.Product;

public class ProductActorProvider : BaseActorProvider<ProductActor>
{
    public ProductActorProvider(ActorSystem actorSystem)
    {
        this.ActorRef = actorSystem.ActorOfWithDI<ProductActor>("product-actor");
    }
}
