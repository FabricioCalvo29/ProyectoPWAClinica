using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica_API.Models
{
    public class RegistrarCitaDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdUsuario { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int IdMedico { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [StringLength(255)]
        public string Motivo { get; set; } = string.Empty;
    }
}
