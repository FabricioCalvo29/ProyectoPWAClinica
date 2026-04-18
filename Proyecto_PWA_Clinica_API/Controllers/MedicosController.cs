using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica_API.Models;
using Proyecto_PWA_Clinica_API.Repositories;

namespace Proyecto_PWA_Clinica_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoRepository _medicoRepository;

        public MedicosController(IMedicoRepository medicoRepository)
        {
            _medicoRepository = medicoRepository;
        }

        [HttpGet("ConsultarEspecialidadesActivas")]
        public async Task<IActionResult> ConsultarEspecialidadesActivas()
        {
            try
            {
                var especialidades = await _medicoRepository.ConsultarEspecialidadesActivas();
                return Ok(especialidades);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = ex.Message
                });
            }
        }

        [HttpGet("ConsultarTodosLosMedicos")]
        public async Task<IActionResult> ConsultarTodosLosMedicos()
        {
            try
            {
                var medicos = await _medicoRepository.ConsultarTodosLosMedicos();
                return Ok(medicos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = ex.Message
                });
            }
        }

        [HttpGet("ConsultarDetalleMedico/{idMedico:int}")]
        public async Task<IActionResult> ConsultarDetalleMedico(int idMedico)
        {
            try
            {
                var medico = await _medicoRepository.ConsultarDetalleMedico(idMedico);
                if (medico == null)
                {
                    return NotFound(new RespuestaApi
                    {
                        EsCorrecto = false,
                        Mensaje = "Médico no encontrado."
                    });
                }

                return Ok(medico);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = ex.Message
                });
            }
        }

        [HttpPost("RegistrarMedicoAdmin")]
        public async Task<IActionResult> RegistrarMedicoAdmin([FromBody] RegistrarMedicoAdminDto model)
        {
            var respuesta = await _medicoRepository.RegistrarMedicoAdmin(model);
            if (respuesta.EsCorrecto)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }

        [HttpPut("ActualizarMedicoAdmin")]
        public async Task<IActionResult> ActualizarMedicoAdmin([FromBody] RegistrarMedicoAdminDto model)
        {
            var respuesta = await _medicoRepository.ActualizarMedicoAdmin(model);
            if (respuesta.EsCorrecto)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }

        [HttpPatch("CambiarEstadoMedicoAdmin/{idMedico:int}/{estado:bool}")]
        public async Task<IActionResult> CambiarEstadoMedicoAdmin(int idMedico, bool estado)
        {
            var respuesta = await _medicoRepository.CambiarEstadoMedicoAdmin(idMedico, estado);
            if (respuesta.EsCorrecto)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }
    }
}