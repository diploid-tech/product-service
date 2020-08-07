using Avanti.ProductService.Product;
using Avanti.ProductService.Product.Api;
using Avanti.ProductService.Product.Documents;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Avanti.ProductServiceTests.Product.Api
{
    public partial class PrivateApiControllerSpec
    {
        public class When_GetProduct_Request_Is_Received : PrivateApiControllerSpec
        {
            [Fact]
            public async void Should_Return_200_When_Found()
            {
                var product = new ProductDocument
                {
                    Description = "Flashlight",
                    Price = 1599,
                    WarehouseId = 2,
                    Properties = new[]
                    {
                        new ProductDocument.Property { Name = "color", Value = "white" }
                    }
                };

                progProductActor.SetResponseForRequest<ProductActor.GetProductById>(request =>
                    new ProductActor.ProductFound { Id = 12, Document = product });

                var result = await Subject.GetProduct(
                    new PrivateApiController.GetProductRequest { Id = 12 });

                result.Should().BeOfType<OkObjectResult>()
                    .Which.Value.Should().BeEquivalentTo(
                        new PrivateApiController.GetProductResponse
                        {
                            Id = 12,
                            Description = "Flashlight",
                            Price = 1599,
                            WarehouseId = 2,
                            Properties = new[]
                            {
                                    new PrivateApiController.GetProductResponse.Property { Name = "color", Value = "white" }
                            }
                        });

                progProductActor.GetRequest<ProductActor.GetProductById>()
                    .Should().BeEquivalentTo(
                        new ProductActor.GetProductById { Id = 12 });
            }

            [Fact]
            public async void Should_Return_404_When_Not_Found()
            {
                progProductActor.SetResponseForRequest<ProductActor.GetProductById>(request =>
                    new ProductActor.ProductNotFound());

                var result = await Subject.GetProduct(
                    new PrivateApiController.GetProductRequest { Id = 12 });

                result.Should().BeOfType<NotFoundResult>();
            }

            [Fact]
            public async void Should_Return_500_When_Failure_To_Retrieve()
            {
                progProductActor.SetResponseForRequest<ProductActor.GetProductById>(request =>
                    new ProductActor.ProductRetrievalFailed());

                var result = await Subject.GetProduct(
                    new PrivateApiController.GetProductRequest { Id = 12 });

                result.Should().BeOfType<StatusCodeResult>()
                    .Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
