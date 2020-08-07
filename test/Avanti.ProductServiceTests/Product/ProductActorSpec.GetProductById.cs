using System.Text.Json;
using Akka.Actor;
using Avanti.Core.RelationalData;
using Avanti.ProductService.Product;
using Avanti.ProductService.Product.Documents;
using FluentAssertions;
using Xunit;

namespace Avanti.ProductServiceTests.Product
{
    public partial class ProductActorSpec
    {
        public class When_Get_Product_By_Id_Is_Received : ProductActorSpec
        {
            private ProductActor.GetProductById input = new ProductActor.GetProductById
            {
                Id = 12
            };

            [Fact]
            public void Should_Return_Product_When_Found()
            {
                var document = new ProductDocument
                {
                    Description = "shirt",
                    Price = 13000,
                    WarehouseId = 2,
                    Properties =
                    new[]
                    {
                            new ProductDocument.Property { Name = "color", Value = "blue" },
                            new ProductDocument.Property { Name = "size", Value = "M" }
                    }
                };

                this.progDatastoreActor.SetResponseForRequest<RelationalDataStoreActor.ExecuteScalar>(request =>
                        new RelationalDataStoreActor.ScalarResult(JsonSerializer.Serialize(document)));

                Subject.Tell(input);

                Kit.ExpectMsg<ProductActor.ProductFound>().Should().BeEquivalentTo(
                    new ProductActor.ProductFound
                    {
                        Id = 12,
                        Document = document
                    });

                this.progDatastoreActor.GetRequest<RelationalDataStoreActor.ExecuteScalar>()
                    .Should().BeEquivalentTo(new RelationalDataStoreActor.ExecuteScalar(
                        DataStoreStatements.GetProductById,
                        new
                        {
                            Id = 12
                        }));
            }

            [Fact]
            public void Should_Return_Not_Found_When_Not_Found()
            {
                this.progDatastoreActor.SetResponseForRequest<RelationalDataStoreActor.ExecuteScalar>(request =>
                    new RelationalDataStoreActor.ScalarResult(null));

                Subject.Tell(input);

                Kit.ExpectMsg<ProductActor.ProductNotFound>();
            }

            [Fact]
            public void Should_Return_Failure_When_Could_Not_Retrieve_Data()
            {
                this.progDatastoreActor.SetResponseForRequest<RelationalDataStoreActor.ExecuteScalar>(request =>
                    new RelationalDataStoreActor.ExecuteFailed());

                Subject.Tell(input);

                Kit.ExpectMsg<ProductActor.ProductRetrievalFailed>();
            }
        }
    }
}
