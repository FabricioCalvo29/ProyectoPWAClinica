namespace Proyecto_PWA_Clinica_API.Models
{
    public class TokenRecuperacionDto
    {
        public int IdCredencial { get; set; }
        public int IdUsuario { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string TokenRecuperacion { get; set; } = string.Empty;
        public DateTime FechaVencimientoToken { get; set; }
    }
}