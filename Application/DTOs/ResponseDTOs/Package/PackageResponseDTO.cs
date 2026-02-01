using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDTOs.Package
{
    public class PackageResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? DurationDays { get; set; }
        public List<string>? Features { get; set; }
    }
}
