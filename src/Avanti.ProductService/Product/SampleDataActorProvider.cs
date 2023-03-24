using Avanti.Core.Microservice.Actors;
using Avanti.Core.Microservice.Extensions;

namespace Avanti.ProductService.Product;

public class SampleDataActorProvider : BaseActorProvider<SampleDataActor>
{
    public SampleDataActorProvider(ActorSystem actorSystem)
    {
        this.ActorRef = actorSystem.ActorOfWithDI<SampleDataActor>("sample-actor");
    }
}
