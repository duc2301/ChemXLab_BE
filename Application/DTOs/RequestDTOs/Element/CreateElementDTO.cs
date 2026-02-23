using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDTOs.Element
{
    /// <summary>
    /// Data Transfer Object for creating a new chemical element.
    /// </summary>
    public class CreateElementDTO
    {
        /// <summary>
        /// The chemical symbol of the element (e.g., "H", "He", "Li").
        /// </summary>
        [Required(ErrorMessage = "Symbol is required")]
        [StringLength(3, MinimumLength = 1, ErrorMessage = "Symbol must be 1-3 characters")]
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// The full name of the element (e.g., "Hydrogen", "Helium").
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be 1-50 characters")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The atomic mass of the element.
        /// </summary>
        public decimal? AtomicMass { get; set; }

        /// <summary>
        /// Properties stored as JSON string including group, period, category, color_hex, etc.
        /// </summary>
        public string? Properties { get; set; }
    }
}
