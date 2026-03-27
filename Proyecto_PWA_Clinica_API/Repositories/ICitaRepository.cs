using Proyecto_PWA_Clinica_API.Models;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public interface ICitaRepository
    {
        Task<IEnumerable<MedicoDisponibleDto>> ConsultarMedicosActivosParaCita();
        Task<RespuestaApi> RegistrarCitaPaciente(RegistrarCitaDto model);
        Task<IEnumerable<CitaDto>> ConsultarCitasPaciente(int idUsuario);
        Task<CitaDto?> ConsultarDetalleCitaPaciente(int idUsuario, int idCita);
    }
}
