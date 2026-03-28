namespace Proyecto_PWA_Clinica_API.Models
{
    public class PacienteListaDto
    {
        public int IdPaciente { get; set; }
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? TipoSangre { get; set; }
        public bool Estado { get; set; }
    }
}
