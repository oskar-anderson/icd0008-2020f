using System.Collections.Generic;
using Domain;

namespace WebApp.DTO
{
    public class ServiceDTO
    {
        public string ID { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Price { get; set; }
        public bool IsPartsReady { get; set; }
        List<ServiceStockPart> ServiceStockParts { get;set; } = default!;
    }
}