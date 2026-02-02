using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Package
{
    /// <summary>
    /// Data Transfer Object for updating an existing package.
    /// </summary>
    public class UpdatePackageDTO
    {
        /// <summary>
        /// The new display name of the package.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The new price of the package.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// The new duration in days.
        /// </summary>
        public int? DurationDays { get; set; }

        /// <summary>
        /// The updated list of features.
        /// </summary>
        public List<string>? Features { get; set; }
    }
}