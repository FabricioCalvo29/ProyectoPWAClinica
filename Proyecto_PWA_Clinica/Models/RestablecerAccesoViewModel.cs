using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica.Models
{
    public class RestablecerAccesoViewModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NuevaContrasena { get; set; } = string.Empty;

        [Required]
        [Compare("NuevaContrasena")]
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}