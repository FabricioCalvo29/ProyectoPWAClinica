using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica_API.Models;
using Proyecto_PWA_Clinica_API.Repositories;

namespace Proyecto_PWA_Clinica_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet("EstadisticasAdmin")]
        public async Task<IActionResult> EstadisticasAdmin()
        {
            try
            {
                var stats = await _dashboardRepository.ConsultarEstadisticasAdmin();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi { EsCorrecto = false, Mensaje = ex.Message });
            }
        }

        [HttpGet("EstadisticasPaciente/{idUsuario:int}")]
        public async Task<IActionResult> EstadisticasPaciente(int idUsuario)
        {
            try
            {
                var stats = await _dashboardRepository.ConsultarEstadisticasPaciente(idUsuario);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi { EsCorrecto = false, Mensaje = ex.Message });
            }
        }
        [HttpGet("EstadisticasMedico/{idUsuario:int}")]
        public async Task<IActionResult> EstadisticasMedico(int idUsuario)
        {
            try
            {
                var stats = await _dashboardRepository.ConsultarEstadisticasMedico(idUsuario);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi { EsCorrecto = false, Mensaje = ex.Message });
            }
        }
    }
}
