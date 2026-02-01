using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.Package
{
    public class CreatePackageDTO
    {
        [Required]
        public string Name { get; set; } = null!;
        public decimal? Price { get; set; }
        public int? DurationDays { get; set; }
        public List<string>? Features { get; set; }
    }
}
