namespace Proyecto_PWA_Clinica_API.Models
{
    public class MedicoDisponibleDto
    {
        public int IdMedico { get; set; }
        public string NombreMedico { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
    }
}
