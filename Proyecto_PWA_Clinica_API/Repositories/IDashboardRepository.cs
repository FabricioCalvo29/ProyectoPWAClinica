using Proyecto_PWA_Clinica_API.Models;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public interface IDashboardRepository
    {
        Task<EstadisticasAdminDto> ConsultarEstadisticasAdmin();
        Task<EstadisticasPacienteDto> ConsultarEstadisticasPaciente(int idUsuario);
    }
}
