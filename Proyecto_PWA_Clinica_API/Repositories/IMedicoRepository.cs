using Proyecto_PWA_Clinica_API.Models;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public interface IMedicoRepository
    {
        Task<IEnumerable<EspecialidadDto>> ConsultarEspecialidadesActivas();
        Task<IEnumerable<MedicoAdminDto>> ConsultarTodosLosMedicos();
        Task<MedicoAdminDto?> ConsultarDetalleMedico(int idMedico);
        Task<RespuestaApi> RegistrarMedicoAdmin(RegistrarMedicoAdminDto model);
        Task<RespuestaApi> ActualizarMedicoAdmin(RegistrarMedicoAdminDto model);
        Task<RespuestaApi> CambiarEstadoMedicoAdmin(int idMedico, bool estado);
    }
}