using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica.Models
{
    public class RecuperarAccesoViewModel
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;
    }
}