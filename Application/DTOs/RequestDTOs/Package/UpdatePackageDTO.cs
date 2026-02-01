using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Package
{
    public class UpdatePackageDTO
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? DurationDays { get; set; }
        public List<string>? Features { get; set; }
    }
}
