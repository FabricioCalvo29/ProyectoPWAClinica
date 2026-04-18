namespace Proyecto_PWA_Clinica_API.Models
{
    public class RegistrarMedicoAdminDto
    {
        public int IdMedico { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasenna { get; set; } = string.Empty;
        public int IdEspecialidad { get; set; }
        public string CodigoProfesional { get; set; } = string.Empty;
    }
}