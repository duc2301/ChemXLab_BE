using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs.RequestDTOs.Package
{
    /// <summary>
    /// Data Transfer Object for creating a new subscription package.
    /// </summary>
    public class CreatePackageDTO
    {
        /// <summary>
        /// The display name of the package.
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The cost of the package (e.g., in VND).
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// The validity period of the package in days.
        /// </summary>
        public int? DurationDays { get; set; }

        /// <summary>
        /// A list of features included in this package.
        /// </summary>
        public List<string>? Features { get; set; }
    }
}