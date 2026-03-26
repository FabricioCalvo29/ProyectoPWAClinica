using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica_API.Models;
using Proyecto_PWA_Clinica_API.Repositories;
using Proyecto_PWA_Clinica_API.Services;

namespace Proyecto_PWA_Clinica_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeguridadController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUtilitario _utilitario;
        private readonly IConfiguration _configuration;

        public SeguridadController(
            IUsuarioRepository usuarioRepository,
            IUtilitario utilitario,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _utilitario = utilitario;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("RecuperarAcceso")]
        public async Task<IActionResult> RecuperarAcceso([FromBody] RecuperarAccesoRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "Los datos enviados no son válidos."
                });
            }

            var existe = await _usuarioRepository.ValidarCorreoExiste(model.Correo);

            if (!existe)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "El correo indicado no se encuentra registrado."
                });
            }

            var token = Guid.NewGuid().ToString("N");
            var vencimiento = DateTime.Now.AddMinutes(30);

            var guardado = await _usuarioRepository.GuardarTokenRecuperacion(model.Correo, token, vencimiento);

            if (!guardado)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "No fue posible guardar el token de recuperación."
                });
            }
            var urlWeb = _configuration["Valores:UrlWeb"] ?? "";
            var enlace = $"{urlWeb}Home/RestablecerAcceso?token={token}";

            var contenido = $@"
                <h2>Recuperación de acceso</h2>
                <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                <p>Haz clic en el siguiente enlace para continuar:</p>
                <p><a href='{enlace}'>Restablecer contraseña</a></p>
                <p>Este enlace vence en 30 minutos.</p>
            ";

            _utilitario.EnviarCorreo(model.Correo, "Recuperación de acceso - Clínica CR", contenido);

            return Ok(new RespuestaApi
            {
                EsCorrecto = true,
                Mensaje = "Se ha enviado un correo con las instrucciones de recuperación."
            });
        }

        [AllowAnonymous]
        [HttpPut("RestablecerAcceso")]
        public async Task<IActionResult> RestablecerAcceso([FromBody] RestablecerAccesoRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "Los datos enviados no son válidos."
                });
            }

            var tokenValido = await _usuarioRepository.ValidarTokenRecuperacion(model.Token);

            if (tokenValido == null)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "El token no es válido o ya venció."
                });
            }

            var contrasenaEncriptada = _utilitario.Encrypt(model.NuevaContrasena);

            var actualizado = await _usuarioRepository.ActualizarContrasena(
                tokenValido.IdCredencial,
                contrasenaEncriptada
            );

            if (!actualizado)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "No fue posible actualizar la contraseña."
                });
            }

            return Ok(new RespuestaApi
            {
                EsCorrecto = true,
                Mensaje = "La contraseña se actualizó correctamente."
            });
        }
    }
}