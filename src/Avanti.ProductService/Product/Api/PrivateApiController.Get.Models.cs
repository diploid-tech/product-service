using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Avanti.ProductService.Product.Api
{
    public partial class PrivateApiController
    {
        public class GetProductRequest
        {
            [Required]
            public int? Id { get; set; }
        }

        public class GetProductResponse
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
}
