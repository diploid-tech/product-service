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
        [SwaggerResponse(200, "The product is found")]
        [SwaggerResponse(400, "The input is invalid")]
        [SwaggerResponse(404, "The product is not found")]
        [SwaggerOperation(
                    Summary = "Get a product",
                    Description = "Retrieves a product by identifier.",
                    Tags = new[] { "Product" })]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetProduct([FromRoute] GetProductRequest request)
        {
            this.logger.LogDebug($"Incoming request to get product with id '{request.Id}'");

            return await this.productActorRef.Ask<ProductActor.IResponse>(
                new ProductActor.GetProductById { Id = request.Id!.Value }) switch
            {
                ProductActor.ProductFound found => new OkObjectResult(this.mapper.Map<GetProductResponse>(found)),
                ProductActor.ProductNotFound => new NotFoundResult(),
                _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
