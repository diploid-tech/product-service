using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Avanti.ProductService.Product.Documents;

namespace Avanti.ProductService.Product
{
    public partial class ProductActor
    {
        public interface IResponse { }

        public class ProductFound : IResponse
        {
            public int Id { get; set; }
            public ProductDocument Document { get; set; } = new ProductDocument();
        }

        public class ProductNotFound : IResponse { }
        public class ProductRetrievalFailed : IResponse { }

        public class ProductStored : IResponse
        {
            public int Id { get; set; }
        }

        public class ProductFailedToStore : IResponse { }

        public class ProductsFound : IResponse
        {
            public IImmutableDictionary<int, ProductDocument> Products { get; set; } = new Dictionary<int, ProductDocument>().ToImmutableDictionary();
        }
    }
}
