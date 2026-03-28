using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica_API.Models;
using Proyecto_PWA_Clinica_API.Repositories;

namespace Proyecto_PWA_Clinica_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class TratamientosController : ControllerBase
    {
        private readonly ITratamientoRepository _tratamientoRepository;

        public TratamientosController(ITratamientoRepository tratamientoRepository)
        {
            _tratamientoRepository = tratamientoRepository;
        }

        [HttpPost("RegistrarTratamiento")]
        public async Task<IActionResult> RegistrarTratamiento([FromBody] RegistrarTratamientoDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "Los datos enviados no son válidos."
                });
            }

            var respuesta = await _tratamientoRepository.RegistrarTratamiento(model);
            if (respuesta.EsCorrecto)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }

        [HttpGet("ConsultarTratamientosPorCita/{idCita:int}")]
        public async Task<IActionResult> ConsultarTratamientosPorCita(int idCita)
        {
            try
            {
                var tratamientos = await _tratamientoRepository.ConsultarTratamientosPorCita(idCita);
                return Ok(tratamientos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi { EsCorrecto = false, Mensaje = ex.Message });
            }
        }

        [HttpGet("ConsultarTratamientosPaciente/{idUsuario:int}")]
        public async Task<IActionResult> ConsultarTratamientosPaciente(int idUsuario)
        {
            try
            {
                var tratamientos = await _tratamientoRepository.ConsultarTratamientosPaciente(idUsuario);
                return Ok(tratamientos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new RespuestaApi { EsCorrecto = false, Mensaje = ex.Message });
            }
        }
    }
}
