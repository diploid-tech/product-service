using System;
using System.Collections.Generic;
using System.Globalization;
using Avanti.Core.EventStream.Model;
using Newtonsoft.Json;

namespace Avanti.ProductService.Product.Events
{
    [PlatformEventDescription("Avanti.Product.ProductUpdated", EventProcessingTypeEnum.Sending, "product")]
    public class ProductUpdated : PlatformEvent
    {
        public override string SubjectId { get => this.Id.ToString(CultureInfo.InvariantCulture); }
        public int Id { get; set; }
    }
}
