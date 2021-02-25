using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Avanti.ProductService.Product.Api
{
    public partial class PrivateApiController
    {
        [SwaggerResponse(200, "The product is stored")]
        [SwaggerResponse(400, "The product is invalid")]
        [SwaggerOperation(
                    Summary = "Upsert a product",
                    Description = "Insert or update the given product, identified by id.",
                    Tags = new[] { "Product" })]
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] PostProductRequest request)
        {
            this.logger.LogInformation($"Incoming request to upsert product with id '{request.Id}'");

            return await this.productActorRef.Ask<ProductActor.IResponse>(
                this.mapper.Map<ProductActor.UpsertProduct>(request)) switch
            {
                ProductActor.ProductStored stored => new OkObjectResult(new PostProductResponse
                {
                    Id = stored.Id
                }),
                _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
            };
        }

        [SwaggerResponse(200, "The list of products that are found")]
        [SwaggerOperation(
                    Summary = "Get multiple products",
                    Description = "Retrieve details from multiple products in a single call",
                    Tags = new[] { "Product" })]
        [HttpPost("list")]
        public async Task<IActionResult> PostGetMultipleProducts([FromBody] PostMultipleProductsRequest request)
        {
            this.logger.LogDebug($"Incoming request get products '{string.Join(", ", request.ProductIds)}'");

            return await this.productActorRef.Ask<ProductActor.IResponse>(
                new ProductActor.GetProductsById { ProductIds = request.ProductIds }) switch
            {
                ProductActor.ProductsFound found => new OkObjectResult(
                        found.Products.Select(p => this.mapper.Map<PostMultipleProductsResponse>(p))),
                _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
