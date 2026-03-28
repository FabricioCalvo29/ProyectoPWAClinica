using Proyecto_PWA_Clinica_API.Models;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public interface IPacienteRepository
    {
        Task<IEnumerable<PacienteListaDto>> ConsultarTodosLosPacientes();
        Task<PacienteListaDto?> ConsultarDetallePaciente(int idPaciente);
    }
}
