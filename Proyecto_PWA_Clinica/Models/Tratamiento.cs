namespace Proyecto_PWA_Clinica.Models
{
    public class Tratamiento
    {
        public int IdTratamiento { get; set; }
        public int IdCita { get; set; }
        public string Medicamento { get; set; } = string.Empty;
        public string Dosis { get; set; } = string.Empty;
        public string Duracion { get; set; } = string.Empty;
        public string? Instrucciones { get; set; }
    }
}