using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDTOs.Element
{
    /// <summary>
    /// Data Transfer Object for updating a chemical element.
    /// </summary>
    public class UpdateElementDTO
    {
        /// <summary>
        /// The chemical symbol of the element.
        /// </summary>
        [StringLength(3, MinimumLength = 1, ErrorMessage = "Symbol must be 1-3 characters")]
        public string? Symbol { get; set; }

        /// <summary>
        /// The full name of the element.
        /// </summary>
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Name must be 1-50 characters")]
        public string? Name { get; set; }

        /// <summary>
        /// The atomic mass of the element.
        /// </summary>
        public decimal? AtomicMass { get; set; }

        /// <summary>
        /// Properties stored as JSON string.
        /// </summary>
        public string? Properties { get; set; }
    }
}
