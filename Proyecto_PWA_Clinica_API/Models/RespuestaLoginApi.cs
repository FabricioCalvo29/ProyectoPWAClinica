namespace Proyecto_PWA_Clinica_API.Models
{
    public class RespuestaLoginApi
    {
        public bool EsCorrecto { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public UsuarioSesionDto? Usuario { get; set; }
    }
}