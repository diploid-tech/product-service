using System.ComponentModel.DataAnnotations;
using Avanti.Core.Microservice.Web;

namespace Avanti.ProductService.Product.Api;

public partial class PrivateApiController
{
    public class PostProductRequest
    {
        public int? Id { get; set; }

        [Required]
        [MinLength(5)]
        public string Description { get; set; } = "unknown";

        [Required]
        public int? Price { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        [MustHaveElements]
        public IEnumerable<Property> Properties { get; set; } = Array.Empty<Property>();

        public class Property
        {
            [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            public string Value { get; set; } = string.Empty;
        }
    }

    public class PostProductResponse
    {
        public int Id { get; set; }
    }

    public class PostMultipleProductsRequest
    {
        public IEnumerable<int> ProductIds { get; set; } = Array.Empty<int>();
    }

    public class PostMultipleProductsResponse
    {
        public int Id { get; set; }

        public string Description { get; set; } = "unknown";

        public int Price { get; set; }

        public int WarehouseId { get; set; }

        public IEnumerable<Property> Properties { get; set; } = Array.Empty<Property>();

        public class Property
        {
            public string Name { get; set; } = string.Empty;

            public string Value { get; set; } = string.Empty;
        }
    }
}
