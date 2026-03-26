using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica_API.Models
{
    public class UsuarioRegistroDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Contrasena { get; set; } = string.Empty;

        [Required]
        public DateTime FechaNacimiento { get; set; }

        public string? Telefono { get; set; }

        public string? TipoSangre { get; set; }

        public string? HistorialMedico { get; set; }
    }
}