using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.RequestDTOs.Chemical
{
    public class UpdateChemicalDTO
    {
        public string Formula { get; set; } = null!;
        public string? CommonName { get; set; }
        public string? IupacName { get; set; }
        public string? StateAtRoomTemp { get; set; }
        public string? Structure3dUrl { get; set; }
        public object? MolecularData { get; set; }
        public bool? IsPublic { get; set; }
    }
}
