using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica_API.Models
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasena { get; set; } = string.Empty;
    }
}