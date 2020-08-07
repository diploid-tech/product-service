using System;
using System.Globalization;
using System.Text.Json;
using Akka.Actor;
using Avanti.Core.EventStream;
using Avanti.Core.RelationalData;
using Avanti.ProductService.Product;
using Avanti.ProductService.Product.Documents;
using Avanti.ProductService.Product.Events;
using FluentAssertions;
using Xunit;

namespace Avanti.ProductServiceTests.Product
{
    public partial class ProductActorSpec
    {
        public class When_Upsert_Product_Is_Received : ProductActorSpec
        {
            private ProductActor.UpsertProduct input = new ProductActor.UpsertProduct
            {
                Id = 12,
                Description = "shirt",
                Price = 13000,
                WarehouseId = 2,
                Properties =
                    new[]
                    {
                            new ProductActor.UpsertProduct.Property { Name = "color", Value = "blue" },
                            new ProductActor.UpsertProduct.Property { Name = "size", Value = "M" }
                    }
            };

            public When_Upsert_Product_Is_Received()
            {
                this.progDatastoreActor.SetResponseForRequest<RelationalDataStoreActor.ExecuteScalar>(request =>
                    new RelationalDataStoreActor.ScalarResult(12));

                this.progPlatformEventActor.SetResponseForRequest<PlatformEventActor.SendEvent>(request =>
                    new PlatformEventActor.EventSendSuccess());
            }

            [Fact]
            public void Should_Return_Stored_When_Existing_Product_Is_Updated_Successfully_And_Send_Event()
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

                Subject.Tell(input);

                Kit.ExpectMsg<ProductActor.ProductStored>().Should().BeEquivalentTo(
                    new ProductActor.ProductStored
                    {
                        Id = 12
                    });

                this.progDatastoreActor.GetRequest<RelationalDataStoreActor.ExecuteScalar>()
                    .Should().BeEquivalentTo(new RelationalDataStoreActor.ExecuteScalar(
                        DataStoreStatements.UpdateProduct,
                        new
                        {
                            Id = 12,
                            ProductJson = JsonSerializer.Serialize(document),
                            Now = DateTimeOffset.Parse("2018-04-01T07:00:00Z", CultureInfo.InvariantCulture)
                        }));

                this.progPlatformEventActor.GetRequest<PlatformEventActor.SendEvent>().Should().BeEquivalentTo(
                    new PlatformEventActor.SendEvent(
                        new ProductUpdated
                        {
                            Id = 12
                        }));
            }

            [Fact]
            public void Should_Return_Stored_When_New_Product_Is_Inserted_Successfully_And_Send_Event()
            {
                this.progDatastoreActor.SetResponseForRequest<RelationalDataStoreActor.ExecuteScalar>(request =>
                    new RelationalDataStoreActor.ScalarResult(99));

                var document = new ProductDocument
                {
                    Description = "new_shirt",
                    Price = 13000,
                    WarehouseId = 2,
                    Properties = new[]
                    {
                            new ProductDocument.Property { Name = "color", Value = "blue" },
                            new ProductDocument.Property { Name = "size", Value = "M" }
                    }
                };

                Subject.Tell(new ProductActor.UpsertProduct
                {
                    Description = "new_shirt",
                    Price = 13000,
                    WarehouseId = 2,
                    Properties = new[]
                    {
                            new ProductActor.UpsertProduct.Property { Name = "color", Value = "blue" },
                            new ProductActor.UpsertProduct.Property { Name = "size", Value = "M" }
                    }
                });

                Kit.ExpectMsg<ProductActor.ProductStored>().Should().BeEquivalentTo(
                    new ProductActor.ProductStored
                    {
                        Id = 99
                    });

                this.progDatastoreActor.GetRequest<RelationalDataStoreActor.ExecuteScalar>()
                    .Should().BeEquivalentTo(new RelationalDataStoreActor.ExecuteScalar(
                        DataStoreStatements.InsertProduct,
                        new
                        {
                            ProductJson = JsonSerializer.Serialize(document),
                            Now = DateTimeOffset.Parse("2018-04-01T07:00:00Z", CultureInfo.InvariantCulture)
                        }));

                this.progPlatformEventActor.GetRequest<PlatformEventActor.SendEvent>().Should().BeEquivalentTo(
                    new PlatformEventActor.SendEvent(
                        new ProductUpdated
                        {
                            Id = 99
                        }));
            }

            [Fact]
            public void Should_Return_Failure_When_Failed_To_Store()
            {
                this.progDatastoreActor.SetResponseForRequest<RelationalDataStoreActor.ExecuteScalar>(request =>
                    new RelationalDataStoreActor.ExecuteFailed());

                Subject.Tell(input);

                Kit.ExpectMsg<ProductActor.ProductFailedToStore>();
            }

            [Fact]
            public void Should_Return_Failure_When_Failed_To_Send_Event()
            {
                this.progPlatformEventActor.SetResponseForRequest<PlatformEventActor.SendEvent>(request =>
                    new PlatformEventActor.EventSendFailed());

                Subject.Tell(input);

                Kit.ExpectMsg<ProductActor.ProductFailedToStore>();
            }
        }
    }
}
