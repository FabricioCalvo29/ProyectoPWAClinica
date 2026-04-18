using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica.Models
{
    public class RegistrarMedicoViewModel
    {
        public int IdMedico { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Contrasenna { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [Compare("Contrasenna", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasenna { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar una especialidad.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una especialidad.")]
        public int IdEspecialidad { get; set; }

        public string CodigoProfesional { get; set; } = string.Empty;

        public bool Estado { get; set; }
    }
}