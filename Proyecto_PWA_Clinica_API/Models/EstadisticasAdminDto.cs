namespace Proyecto_PWA_Clinica_API.Models
{
    public class EstadisticasAdminDto
    {
        public int TotalUsuarios { get; set; }
        public int CitasPendientes { get; set; }
        public int CitasCompletadas { get; set; }
        public int CitasHoy { get; set; }
    }
}
