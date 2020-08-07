using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using AutoMapper;
using Avanti.Core.EventStream;
using Avanti.Core.Microservice;
using Avanti.Core.RelationalData;
using Avanti.ProductService.Product.Documents;

namespace Avanti.ProductService.Product
{
    public partial class ProductActor : ReceiveActor
    {
        private readonly ILoggingAdapter log = Logging.GetLogger(Context);
        private readonly IRelationalDataStoreActorProvider relationalDataStoreActorProvider;
        private readonly IPlatformEventActorProvider platformEventActorProvider;
        private readonly IMapper mapper;
        private readonly IClock clock;

        public ProductActor(
            IRelationalDataStoreActorProvider datastoreActorProvider,
            IPlatformEventActorProvider platformEventActorProvider,
            IMapper mapper,
            IClock clock)
        {
            this.relationalDataStoreActorProvider = datastoreActorProvider;
            this.platformEventActorProvider = platformEventActorProvider;
            this.mapper = mapper;
            this.clock = clock;

            ReceiveAsync<GetProductById>(m => HandleGetProductById(m).PipeTo(Sender));
            ReceiveAsync<GetProductsById>(m => HandleGetProductsById(m).PipeTo(Sender));
            ReceiveAsync<UpsertProduct>(m => HandleUpsertProduct(m).PipeTo(Sender));
        }

        private async Task<IResponse> HandleGetProductById(GetProductById m)
        {
            this.log.Info($"Incoming request for getting product by id {m.Id}");

            var result = await this.relationalDataStoreActorProvider.ExecuteScalarJsonAs<ProductDocument>(
                DataStoreStatements.GetProductById,
                new
                {
                    Id = m.Id
                });

            return result switch
            {
                IsSome<ProductDocument> scalar => new ProductFound { Id = m.Id, Document = scalar.Value },
                IsNone _ => new ProductNotFound(),
                _ => new ProductRetrievalFailed()
            };
        }

        private async Task<IResponse> HandleGetProductsById(GetProductsById m)
        {
            this.log.Info($"Incoming request for getting products '{string.Join(", ", m.ProductIds)}'");

            var result = await this.relationalDataStoreActorProvider.ExecuteQuery(
                DataStoreStatements.GetProductsById,
                new
                {
                    Ids = m.ProductIds
                });

            return result switch
            {
                IsSome<IEnumerable<dynamic>> records =>
                    new ProductsFound
                    {
                        Products = new Dictionary<int, ProductDocument>(
                            records.Value.Select(r => new KeyValuePair<int, ProductDocument>(r.id, JsonSerializer.Deserialize<ProductDocument>((string)r.productjson)))).ToImmutableDictionary()
                    },
                _ => new ProductRetrievalFailed()
            };
        }

        private async Task<IResponse> HandleUpsertProduct(UpsertProduct m)
        {
            this.log.Info($"Incoming request for upserting product with id {m.Id}");

            var document = mapper.Map<ProductDocument>(m);
            var result = await this.relationalDataStoreActorProvider.ExecuteScalar<int>(
                m.Id.HasValue ? DataStoreStatements.UpdateProduct : DataStoreStatements.InsertProduct,
                new
                {
                    Id = m.Id,
                    ProductJson = JsonSerializer.Serialize(document),
                    Now = this.clock.Now()
                });

            switch (result)
            {
                case IsSome<int> id:
                    var e = mapper.Map<Events.ProductUpdated>((id.Value, document));
                    if (await platformEventActorProvider.SendEvent(e) is IsSuccess)
                    {
                        return new ProductStored { Id = id.Value };
                    }

                    break;

                default:
                    break;
            }

            return new ProductFailedToStore();
        }
    }
}