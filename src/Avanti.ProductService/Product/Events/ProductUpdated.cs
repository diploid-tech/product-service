using System.Globalization;
using Avanti.Core.EventStream.Model;

namespace Avanti.ProductService.Product.Events
{
    [PlatformEventDescription("Avanti.Product.ProductUpdated", EventProcessingTypeEnum.Sending, "product")]
    public class ProductUpdated : PlatformEvent
    {
        public override string SubjectId => this.Id.ToString(CultureInfo.InvariantCulture);
        public int Id { get; set; }
    }
}
