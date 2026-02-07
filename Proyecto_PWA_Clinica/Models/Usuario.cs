using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica.Models
{
    public class Usuario
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Contrasenna { get; set; } = string.Empty;

        // Solo para el registro (confirmación). No lo guardes en BD así.
        [Required(ErrorMessage = "Confirma tu contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Contrasenna", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasenna { get; set; } = string.Empty;
    }
}
