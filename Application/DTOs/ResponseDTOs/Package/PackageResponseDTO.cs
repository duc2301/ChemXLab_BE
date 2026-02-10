using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.ResponseDTOs.Package
{
    /// <summary>
    /// Data Transfer Object representing a subscription package response.
    /// </summary>
    public class PackageResponseDTO
    {
        /// <summary>
        /// The unique identifier of the package.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The display name of the package.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The price of the package.
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// The duration of the package validity in days.
        /// </summary>
        public int? DurationDays { get; set; }

        /// <summary>
        /// A list of features included in the package.
        /// </summary>
        public List<string>? Features { get; set; }
    }
}