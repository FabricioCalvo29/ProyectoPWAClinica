using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica_API.Models
{
    public class RecuperarAccesoRequest
    {
        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;
    }
}