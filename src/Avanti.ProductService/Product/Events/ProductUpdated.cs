using System.Globalization;
using Avanti.Core.EventStream;
using Avanti.Core.EventStream.Model;

namespace Avanti.ProductService.Product.Events;

[PlatformEventDescription("Avanti.Product.ProductUpdated", EventProcessingTypeEnum.Sending, "product")]
public class ProductUpdated : IPlatformEvent
{
    public string SubjectId => this.Id.ToString(CultureInfo.InvariantCulture);
    public int Id { get; set; }
}
