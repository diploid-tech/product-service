using Avanti.ProductService.Product;
using Avanti.ProductService.Product.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Avanti.ProductServiceTests.Product.Api
{
    public partial class PrivateApiControllerSpec
    {
        public class When_PostProduct_Request_Is_Received : PrivateApiControllerSpec
        {
            private readonly PrivateApiController.PostProductRequest request = new()
            {
                Id = 15,
                Description = "Shirt",
                Price = 13000,
                WarehouseId = 2,
                Properties = new[]
                {
                    new PrivateApiController.PostProductRequest.Property { Name = "color", Value = "blue" },
                    new PrivateApiController.PostProductRequest.Property { Name = "size", Value = "M" }
                }
            };

            [Fact]
            public async void Should_Return_200_When_Stored()
            {
                progProductActor.SetResponseForRequest<ProductActor.UpsertProduct>(request =>
                    new ProductActor.ProductStored { Id = 15 });

                IActionResult result = await Subject.PostProduct(request);

                result.Should().BeOfType<OkObjectResult>()
                    .Which.Value.Should().BeEquivalentTo(new PrivateApiController.PostProductResponse { Id = 15 });

                progProductActor.GetRequest<ProductActor.UpsertProduct>()
                    .Should().BeEquivalentTo(
                        new ProductActor.UpsertProduct
                        {
                            Id = 15,
                            Description = "Shirt",
                            Price = 13000,
                            WarehouseId = 2,
                            Properties = new[]
                            {
                                new ProductActor.UpsertProduct.Property { Name = "color", Value = "blue" },
                                new ProductActor.UpsertProduct.Property { Name = "size", Value = "M" }
                            }
                        });
            }

            [Fact]
            public async void Should_Return_500_When_Failed_To_Store()
            {
                progProductActor.SetResponseForRequest<ProductActor.UpsertProduct>(request =>
                    new ProductActor.ProductFailedToStore());

                IActionResult result = await Subject.PostProduct(request);

                result.Should().BeOfType<StatusCodeResult>()
                    .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
