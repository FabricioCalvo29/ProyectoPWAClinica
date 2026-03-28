using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica_API.Models
{
    public class RegistrarTratamientoDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int IdCita { get; set; }

        [Required]
        [StringLength(100)]
        public string Medicamento { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Dosis { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Duracion { get; set; } = string.Empty;

        public string? Instrucciones { get; set; }
    }
}
