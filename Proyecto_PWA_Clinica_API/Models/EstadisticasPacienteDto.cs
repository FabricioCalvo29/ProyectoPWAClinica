namespace Proyecto_PWA_Clinica_API.Models
{
    public class EstadisticasPacienteDto
    {
        public int CitasPendientes { get; set; }
        public int CitasCompletadas { get; set; }
        public int CitasCanceladas { get; set; }
        public int TotalTratamientos { get; set; }
    }
}
