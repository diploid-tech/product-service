using Akka.Actor;
using Akka.DI.Core;
using Avanti.Core.Microservice.Actors;

namespace Avanti.ProductService.Product
{
    public class SampleDataActorProvider : BaseActorProvider<SampleDataActor>
    {
        public SampleDataActorProvider(ActorSystem actorRefFactory) =>
            ActorRef = actorRefFactory.ActorOf(actorRefFactory.DI().Props<SampleDataActor>(), "sample-actor");
    }
}
