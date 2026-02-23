using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.RequestDTOs.Auth
{
    public class GoogleLoginDTO
    {
        [Required(ErrorMessage = "idToken is required")]
        // FE sends Google access_token in this field named idToken
        public string IdToken { get; set; }
    }
}
