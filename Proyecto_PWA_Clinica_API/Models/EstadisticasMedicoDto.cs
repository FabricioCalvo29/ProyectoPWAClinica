namespace Proyecto_PWA_Clinica_API.Models
{
    public class EstadisticasMedicoDto
    {
        public int CitasHoy { get; set; }
        public int CitasPendientes { get; set; }
        public int CitasCompletadas { get; set; }
        public int TotalPacientesAtendidos { get; set; }
    }
}