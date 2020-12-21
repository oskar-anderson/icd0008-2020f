using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ServiceStockPart
    {
        [StringLength(36)]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [StringLength(36)]
        public string StockPartID { get; set; } = default!;
        public StockPart? StockPart { get; set; } = default!;

        [StringLength(36)]
        public string ServiceID { get; set; } = default!;
        public Service? Service { get; set; } = default!;
        
        [Range(1, Int32.MaxValue)]
        public int Quantity { get; set; } 
    }
}