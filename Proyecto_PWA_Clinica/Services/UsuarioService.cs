using Proyecto_PWA_Clinica.Models;
using System.Text;
using System.Text.Json;

namespace Proyecto_PWA_Clinica.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UsuarioService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(bool, string)> RegistrarUsuario(Usuario model)
        {
            var datos = new
            {
                model.Nombre,
                model.Apellido,
                model.Correo,
                Contrasena = model.Contrasena,
                model.Cedula,
                model.FechaNacimiento,
                model.Telefono,
                model.TipoSangre,
                model.HistorialMedico
            };

            var url = _configuration["Valores:UrlAPI"] + "Usuario/RegistrarPaciente";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var mensaje = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return (true, mensaje);

            return (false, mensaje);
        }

        public async Task<(bool, string)> RecuperarAcceso(string correo)
        {
            var datos = new
            {
                Correo = correo
            };

            var url = _configuration["Valores:UrlAPI"] + "Seguridad/RecuperarAcceso";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var mensaje = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, mensaje);
        }

        public async Task<(bool, string)> RestablecerAcceso(string token, string nuevaContrasena, string confirmarContrasena)
        {
            var datos = new
            {
                Token = token,
                NuevaContrasena = nuevaContrasena,
                ConfirmarContrasena = confirmarContrasena
            };

            var url = _configuration["Valores:UrlAPI"] + "Seguridad/RestablecerAcceso";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            var mensaje = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, mensaje);
        }

        public async Task<RespuestaLoginApi> IniciarSesion(Usuario model)
        {
            var datos = new
            {
                model.Correo,
                Contrasena = model.Contrasena
            };

            var url = _configuration["Valores:UrlAPI"] + "Usuario/IniciarSesion";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var respuestaJson = await response.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var resultado = JsonSerializer.Deserialize<RespuestaLoginApi>(respuestaJson, opciones);

            return resultado ?? new RespuestaLoginApi
            {
                EsCorrecto = false,
                Mensaje = "No se pudo interpretar la respuesta del API."
            };
        }
    }
}