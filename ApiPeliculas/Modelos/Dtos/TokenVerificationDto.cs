using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos.Dtos
{
    public class TokenVerificationDto
    {
        [Required(ErrorMessage = "El token es obligatorio")]
        public string Token { get; set; }
    }
} 