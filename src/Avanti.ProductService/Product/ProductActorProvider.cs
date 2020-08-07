using Akka.Actor;
using Akka.DI.Core;
using Avanti.Core.Microservice.Actors;

namespace Avanti.ProductService.Product
{
    public class ProductActorProvider : BaseActorProvider<ProductActor>
    {
        public ProductActorProvider(ActorSystem actorRefFactory) =>
            ActorRef = actorRefFactory.ActorOf(actorRefFactory.DI().Props<ProductActor>(), "product-actor");
    }
}
