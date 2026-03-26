namespace Proyecto_PWA_Clinica.Models
{
    public class RespuestaLoginApi
    {
        public bool EsCorrecto { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public UsuarioSesion? Usuario { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}