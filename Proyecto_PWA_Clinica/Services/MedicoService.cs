using Proyecto_PWA_Clinica.Models;
using System.Text;
using System.Text.Json;

namespace Proyecto_PWA_Clinica.Services
{
    public class MedicoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public MedicoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Especialidad>> ConsultarEspecialidadesActivas()
        {
            var url = _configuration["Valores:UrlAPI"] + "Medicos/ConsultarEspecialidadesActivas";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Especialidad>();

            var json = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Especialidad>>(json, opciones) ?? new List<Especialidad>();
        }

        public async Task<List<MedicoAdmin>> ConsultarTodosLosMedicos()
        {
            var url = _configuration["Valores:UrlAPI"] + "Medicos/ConsultarTodosLosMedicos";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<MedicoAdmin>();

            var json = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<MedicoAdmin>>(json, opciones) ?? new List<MedicoAdmin>();
        }

        public async Task<RegistrarMedicoViewModel?> ConsultarDetalleMedico(int idMedico)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Medicos/ConsultarDetalleMedico/{idMedico}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var medico = JsonSerializer.Deserialize<MedicoAdmin>(json, opciones);
            if (medico == null)
                return null;

            return new RegistrarMedicoViewModel
            {
                IdMedico = medico.IdMedico,
                Nombre = medico.Nombre,
                Apellido = medico.Apellido,
                Correo = medico.Correo,
                IdEspecialidad = medico.IdEspecialidad,
                CodigoProfesional = medico.CodigoProfesional,
                Estado = medico.Estado
            };
        }

        public async Task<(bool, string)> RegistrarMedico(RegistrarMedicoViewModel model)
        {
            var datos = new
            {
                model.Nombre,
                model.Apellido,
                model.Correo,
                model.Contrasenna,
                model.IdEspecialidad
            };

            var url = _configuration["Valores:UrlAPI"] + "Medicos/RegistrarMedicoAdmin";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var respuestaJson = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, ExtraerMensajeRespuesta(respuestaJson));
        }

        public async Task<(bool, string)> ActualizarMedico(RegistrarMedicoViewModel model)
        {
            var datos = new
            {
                model.IdMedico,
                model.Nombre,
                model.Apellido,
                model.Correo,
                model.IdEspecialidad,
                model.CodigoProfesional
            };

            var url = _configuration["Valores:UrlAPI"] + "Medicos/ActualizarMedicoAdmin";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            var respuestaJson = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, ExtraerMensajeRespuesta(respuestaJson));
        }

        public async Task<(bool, string)> CambiarEstadoMedico(int idMedico, bool estado)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Medicos/CambiarEstadoMedicoAdmin/{idMedico}/{estado}";
            var response = await _httpClient.PatchAsync(url, null);
            var respuestaJson = await response.Content.ReadAsStringAsync();

            return (response.IsSuccessStatusCode, ExtraerMensajeRespuesta(respuestaJson));
        }

        private static string ExtraerMensajeRespuesta(string respuestaJson)
        {
            if (string.IsNullOrWhiteSpace(respuestaJson))
                return string.Empty;

            try
            {
                using var document = JsonDocument.Parse(respuestaJson);
                var root = document.RootElement;

                if (root.TryGetProperty("mensaje", out var mensaje))
                    return mensaje.GetString() ?? string.Empty;

                if (root.TryGetProperty("Mensaje", out mensaje))
                    return mensaje.GetString() ?? string.Empty;
            }
            catch
            {
                return respuestaJson;
            }

            return respuestaJson;
        }
    }
}