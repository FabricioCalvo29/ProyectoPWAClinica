namespace Proyecto_PWA_Clinica_API.Models
{
    public class EspecialidadDto
    {
        public int IdEspecialidad { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}