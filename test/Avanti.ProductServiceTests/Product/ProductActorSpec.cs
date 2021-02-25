using System;
using System.Globalization;
using Akka.Actor;
using AutoMapper;
using Avanti.Core.EventStream;
using Avanti.Core.Microservice;
using Avanti.Core.RelationalData;
using Avanti.Core.Unittests;
using Avanti.ProductService.Product;
using Avanti.ProductService.Product.Mappings;

namespace Avanti.ProductServiceTests.Product
{
    public partial class ProductActorSpec : WithSubject<IActorRef>
    {
        private readonly ProgrammableActor<RelationalDataStoreActor> progDatastoreActor;
        private readonly ProgrammableActor<PlatformEventActor> progPlatformEventActor;

        private ProductActorSpec()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ProductMapping()));
            config.AssertConfigurationIsValid();

            progDatastoreActor = Kit.CreateProgrammableActor<RelationalDataStoreActor>();
            var relationalDataStoreActorProvider = new RelationalDataStoreActorProvider(progDatastoreActor.TestProbe);

            progPlatformEventActor = Kit.CreateProgrammableActor<PlatformEventActor>();
            var platformEventActorProvider = new PlatformEventActorProvider(progPlatformEventActor.TestProbe);

            var clock = new FakeClock(DateTimeOffset.Parse("2018-04-01T07:00:00Z", CultureInfo.InvariantCulture));

            Subject = Sys.ActorOf(
                Props.Create<ProductActor>(
                    relationalDataStoreActorProvider,
                    platformEventActorProvider,
                    config.CreateMapper(),
                    clock));
        }
    }
}
