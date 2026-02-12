using System;
using System.Text.Json;

namespace Application.DTOs.ResponseDTOs.Element
{
    /// <summary>
    /// Data Transfer Object for Element response.
    /// </summary>
    public class ElementResponseDTO
    {
        /// <summary>
        /// The unique identifier of the element.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The chemical symbol of the element.
        /// </summary>
        public string Symbol { get; set; } = null!;

        /// <summary>
        /// The full name of the element.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The atomic mass of the element.
        /// </summary>
        public decimal? AtomicMass { get; set; }

        /// <summary>
        /// Properties as a parsed JSON object.
        /// </summary>
        public JsonElement? Properties { get; set; }
    }
}
