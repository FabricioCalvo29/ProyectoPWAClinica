using Proyecto_PWA_Clinica_API.Models;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public interface ITratamientoRepository
    {
        Task<RespuestaApi> RegistrarTratamiento(RegistrarTratamientoDto model);
        Task<IEnumerable<TratamientoDto>> ConsultarTratamientosPorCita(int idCita);
        Task<IEnumerable<TratamientoDto>> ConsultarTratamientosPaciente(int idUsuario);
    }
}
