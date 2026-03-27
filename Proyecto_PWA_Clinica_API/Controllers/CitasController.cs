using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica_API.Models;
using Proyecto_PWA_Clinica_API.Repositories;

namespace Proyecto_PWA_Clinica_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CitasController : ControllerBase
    {
        private readonly ICitaRepository _citaRepository;

        public CitasController(ICitaRepository citaRepository)
        {
            _citaRepository = citaRepository;
        }

        [HttpGet("ConsultarMedicosActivosParaCita")]
        public async Task<IActionResult> ConsultarMedicosActivosParaCita()
        {
            try
            {
                var medicos = await _citaRepository.ConsultarMedicosActivosParaCita();
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

        [HttpPost("RegistrarCitaPaciente")]
        public async Task<IActionResult> RegistrarCitaPaciente([FromBody] RegistrarCitaDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "Los datos enviados no son validos."
                });
            }

            var respuesta = await _citaRepository.RegistrarCitaPaciente(model);

            if (respuesta.EsCorrecto)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }

        [HttpGet("ConsultarCitasPaciente/{idUsuario:int}")]
        public async Task<IActionResult> ConsultarCitasPaciente(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "El IdUsuario enviado no es valido."
                });
            }

            try
            {
                var citas = await _citaRepository.ConsultarCitasPaciente(idUsuario);
                return Ok(citas);
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

        [HttpGet("ConsultarDetalleCitaPaciente/{idUsuario:int}/{idCita:int}")]
        public async Task<IActionResult> ConsultarDetalleCitaPaciente(int idUsuario, int idCita)
        {
            if (idUsuario <= 0 || idCita <= 0)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "Los identificadores enviados no son validos."
                });
            }

            try
            {
                var cita = await _citaRepository.ConsultarDetalleCitaPaciente(idUsuario, idCita);

                if (cita == null)
                {
                    return BadRequest(new RespuestaApi
                    {
                        EsCorrecto = false,
                        Mensaje = "La cita solicitada no existe para el usuario indicado."
                    });
                }

                return Ok(cita);
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
    }
}
