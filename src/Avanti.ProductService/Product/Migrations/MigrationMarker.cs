using Avanti.Core.RelationalData;

namespace Avanti.ProductService.Product.Migrations
{
    public class MigrationMarker : IMigrationMarker
    {
        public string Schema => "product";
    }
}
