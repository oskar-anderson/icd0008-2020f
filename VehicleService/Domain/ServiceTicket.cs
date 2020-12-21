using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ServiceTicket
    {
        [StringLength(36)]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        [StringLength(36)]
        public string ServiceID { get; set; } = default!;
        public Service? Service { get; set; }
        
        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string Vehicle { get; set; } = default!;

        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string RequestedBy { get; set; } = default!;
        
        public bool IsDone { get; set; } = default!;
        
        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string MechanicsNames { get; set; } = default!;

        [MaxLength(256)]
        [MinLength(2)]
        [Required]
        public string Comment { get; set; } = default!;
    }
}