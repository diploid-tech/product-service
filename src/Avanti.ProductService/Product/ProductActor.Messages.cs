using System;
using System.Collections.Generic;

namespace Avanti.ProductService.Product
{
    public partial class ProductActor
    {
        public class GetProductById
        {
            public int Id { get; set; }
        }

        public class GetProductsById
        {
            public IEnumerable<int> ProductIds { get; set; } = Array.Empty<int>();
        }

        public class UpsertProduct
        {
            public int? Id { get; set; }

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
