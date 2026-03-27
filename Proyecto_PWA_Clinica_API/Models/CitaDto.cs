namespace Proyecto_PWA_Clinica_API.Models
{
    public class CitaDto
    {
        public int IdCita { get; set; }
        public int IdPaciente { get; set; }
        public int IdMedico { get; set; }
        public DateTime FechaHora { get; set; }
        public string EstadoCita { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
        public string? NotasMedico { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
    }
}
