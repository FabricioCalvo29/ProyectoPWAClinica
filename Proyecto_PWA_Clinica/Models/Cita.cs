using System.ComponentModel.DataAnnotations;

namespace Proyecto_PWA_Clinica.Models
{
    public class Cita
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un medico.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un medico.")]
        public int IdMedico { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha y hora de la cita.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime FechaHora { get; set; }

        public string EstadoCita { get; set; } = string.Empty;

        [Required(ErrorMessage = "El motivo es obligatorio.")]
        [StringLength(255, ErrorMessage = "El motivo no puede superar los 255 caracteres.")]
        public string Motivo { get; set; } = string.Empty;

        public string? NotasMedico { get; set; }
        public string NombrePaciente { get; set; } = string.Empty;
        public string NombreMedico { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
    }
}