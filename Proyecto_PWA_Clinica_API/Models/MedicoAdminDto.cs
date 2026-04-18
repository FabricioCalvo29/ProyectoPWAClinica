namespace Proyecto_PWA_Clinica_API.Models
{
    public class MedicoAdminDto
    {
        public int IdMedico { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public int IdEspecialidad { get; set; }
        public string Especialidad { get; set; } = string.Empty;
        public string CodigoProfesional { get; set; } = string.Empty;
        public string? CorreoProfesional { get; set; }
        public bool Estado { get; set; }
    }
}