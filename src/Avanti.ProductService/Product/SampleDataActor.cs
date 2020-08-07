using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Avanti.Core.Microservice;
using Avanti.Core.Microservice.Actors;
using Avanti.Core.RelationalData;

namespace Avanti.ProductService.Product
{
    public partial class SampleDataActor : ReceiveActor
    {
        private const int NumberOfSampleProducts = 300;
        private readonly ILoggingAdapter log = Logging.GetLogger(Context);
        private readonly IActorRef productActor;
        private readonly IActorRef datastoreActor;
        private readonly IClock clock;
        private readonly Random random = new Random();

        public SampleDataActor(
            IRelationalDataStoreActorProvider datastoreActorProvider,
            IActorProvider<ProductActor> productActorProvider,
            IClock clock)
        {
            this.productActor = productActorProvider.Get();
            this.datastoreActor = datastoreActorProvider.Get();
            this.clock = clock;

            ReceiveAsync<DataStoreCreatorActor.DataStoreIsInitialized>(_ => Handle());
        }

        private async Task Handle()
        {
            var result = await this.datastoreActor.Ask(new RelationalDataStoreActor.ExecuteQuery(DataStoreStatements.GetSampleDataStatus));

            switch (result)
            {
                case RelationalDataStoreActor.QueryResult r when r.Result.Any():
                    this.log.Info("Sample data already loaded");
                    break;

                case RelationalDataStoreActor.QueryResult _:
                    for (var idx = 0; idx < NumberOfSampleProducts; idx++)
                    {
                        this.productActor.Tell(ConstructUpsertMessage(
                            $"shirt model {Guid.NewGuid()}",
                            this.random.Next(1000, 9999),
                            this.random.Next(1, 3),
                            new Dictionary<string, string> { { "color", "red" } }));
                    }

                    this.datastoreActor.Tell(new RelationalDataStoreActor.ExecuteCommand(
                        DataStoreStatements.InsertSampleDataStatus,
                        new
                        {
                            Now = this.clock.Now()
                        }));

                    this.log.Info("Sample data loaded");
                    break;

                default:
                    this.log.Error("Cannot load sample data!");
                    break;
            }
        }

        private static ProductActor.UpsertProduct ConstructUpsertMessage(string description, int price, int warehouseId, IDictionary<string, string> properties) =>
            new ProductActor.UpsertProduct
            {
                Description = description,
                Price = price,
                WarehouseId = warehouseId,
                Properties = properties.Select(p => new ProductActor.UpsertProduct.Property { Name = p.Key, Value = p.Value })
            };

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(DataStoreCreatorActor.DataStoreIsInitialized));
        }
    }
}
