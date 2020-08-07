using System;
using System.Collections.Generic;

namespace Avanti.ProductService.Product.Documents
{
    public class ProductDocument
    {
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
