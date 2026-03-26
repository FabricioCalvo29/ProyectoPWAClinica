using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Proyecto_PWA_Clinica_API.Models;
using Proyecto_PWA_Clinica_API.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Proyecto_PWA_Clinica_API.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public UsuarioController(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        [HttpPost("RegistrarPaciente")]
        public async Task<IActionResult> RegistrarPaciente([FromBody] UsuarioRegistroDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "Los datos enviados no son válidos."
                });
            }

            var respuesta = await _usuarioRepository.RegistrarUsuario(model);

            if (respuesta.EsCorrecto)
                return Ok(respuesta);

            return BadRequest(respuesta);
        }

        [HttpPost("IniciarSesion")]
        public async Task<IActionResult> IniciarSesion([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginRespuestaDto
                {
                    EsCorrecto = false,
                    Mensaje = "Los datos enviados no son válidos."
                });
            }

            try
            {
                var usuario = await _usuarioRepository.IniciarSesion(model);

                if (usuario == null)
                {
                    return BadRequest(new LoginRespuestaDto
                    {
                        EsCorrecto = false,
                        Mensaje = "Correo o contraseña incorrectos."
                    });
                }

                var token = GenerarToken(usuario);

                return Ok(new LoginRespuestaDto
                {
                    EsCorrecto = true,
                    Mensaje = "Inicio de sesión correcto.",
                    Usuario = usuario,
                    Token = token
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new LoginRespuestaDto
                {
                    EsCorrecto = false,
                    Mensaje = ex.Message
                });
            }
        }

        private string GenerarToken(UsuarioSesionDto usuario)
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("No se encontró Jwt:Key.");

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var claims = new List<Claim>
            {
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                new Claim(ClaimTypes.Email, usuario.Correo)
            };

            foreach (var rol in usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.NombreRol));
            }

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256
            );

            var tokenDescriptor = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}