namespace Proyecto_PWA_Clinica_API.Models
{
    public class UsuarioSesionDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public int IdCredencial { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string TipoAcceso { get; set; } = string.Empty;
        public bool EsPrincipal { get; set; }
        public bool EstadoUsuario { get; set; }
        public bool EstadoCredencial { get; set; }
        public List<RolDto> Roles { get; set; } = new();
    }
}