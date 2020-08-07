using System;
using System.Globalization;
using Akka.Actor;
using AutoMapper;
using Avanti.Core.EventStream;
using Avanti.Core.Microservice;
using Avanti.Core.Microservice.Middleware;
using Avanti.Core.RelationalData;
using Avanti.Core.Unittests;
using Avanti.ProductService.Product;
using Avanti.ProductService.Product.Mappings;
using NSubstitute;

namespace Avanti.ProductServiceTests.Product
{
    public partial class ProductActorSpec : WithSubject<IActorRef>
{
    private ProgrammableActor<RelationalDataStoreActor> progDatastoreActor;
    private ProgrammableActor<PlatformEventActor> progPlatformEventActor;

    private ProductActorSpec()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ProductMapping());
        });
        config.AssertConfigurationIsValid();

        this.progDatastoreActor = Kit.CreateProgrammableActor<RelationalDataStoreActor>();
        var relationalDataStoreActorProvider = new RelationalDataStoreActorProvider(this.progDatastoreActor.TestProbe);

        this.progPlatformEventActor = Kit.CreateProgrammableActor<PlatformEventActor>();
        var platformEventActorProvider = new PlatformEventActorProvider(this.progPlatformEventActor.TestProbe);

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
