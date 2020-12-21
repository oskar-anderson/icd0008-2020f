using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class StockPart
    {
        [StringLength(36)]
        public string ID { get; set; } =  Guid.NewGuid().ToString();
        
        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string Name { get; set; } = default!;
        
        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string Category { get; set; } = default!;
        
        [Range(0, Int32.MaxValue)]
        public int CurrentQuantity { get; set; } = default!;
        
        [Range(0, Int32.MaxValue)]
        public int OptimalQuantity { get; set; } = default!;
        
        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string Location { get; set; } = default!;
        
        [Range(0, Int32.MaxValue)]
        public int Price { get; set; } = default!;
    }
}