using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Service
    {
        [StringLength(36)]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        
        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string Name { get; set; } = default!;
        
        [Range(0, Int32.MaxValue)]
        public int Price { get; set; } = default!;
    }
}