namespace Proyecto_PWA_Clinica_API.Models
{
    public class LoginRespuestaDto
    {
        public bool EsCorrecto { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public UsuarioSesionDto? Usuario { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}