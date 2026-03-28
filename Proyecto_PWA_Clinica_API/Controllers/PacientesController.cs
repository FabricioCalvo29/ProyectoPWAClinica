using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica_API.Models;
using Proyecto_PWA_Clinica_API.Repositories;

namespace Proyecto_PWA_Clinica_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteRepository _pacienteRepository;

        public PacientesController(IPacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        [HttpGet("ConsultarTodosLosPacientes")]
        public async Task<IActionResult> ConsultarTodosLosPacientes()
        {
            try
            {
                var pacientes = await _pacienteRepository.ConsultarTodosLosPacientes();
                return Ok(pacientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi { EsCorrecto = false, Mensaje = ex.Message });
            }
        }

        [HttpGet("ConsultarDetallePaciente/{idPaciente:int}")]
        public async Task<IActionResult> ConsultarDetallePaciente(int idPaciente)
        {
            try
            {
                var paciente = await _pacienteRepository.ConsultarDetallePaciente(idPaciente);
                if (paciente == null)
                    return NotFound(new RespuestaApi { EsCorrecto = false, Mensaje = "Paciente no encontrado" });

                return Ok(paciente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi { EsCorrecto = false, Mensaje = ex.Message });
            }
        }
    }
}
