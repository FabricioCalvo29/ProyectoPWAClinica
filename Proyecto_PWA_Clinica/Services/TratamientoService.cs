using Proyecto_PWA_Clinica.Models;
using System.Text.Json;
using System.Text;

namespace Proyecto_PWA_Clinica.Services
{
    public class TratamientoService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public TratamientoService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(bool, string)> RegistrarTratamiento(Tratamiento model)
        {
            var datos = new
            {
                model.IdCita,
                model.Medicamento,
                model.Dosis,
                model.Duracion,
                model.Instrucciones
            };

            var url = _configuration["Valores:UrlAPI"] + "Tratamientos/RegistrarTratamiento";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var respuestaJson = await response.Content.ReadAsStringAsync();

            var mensaje = ExtraerMensajeRespuesta(respuestaJson);
            return (response.IsSuccessStatusCode, mensaje);
        }

        public async Task<List<Tratamiento>> ConsultarTratamientosPorCita(int idCita)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Tratamientos/ConsultarTratamientosPorCita/{idCita}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Tratamiento>();

            var respuestaJson = await response.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Tratamiento>>(respuestaJson, opciones) ?? new List<Tratamiento>();
        }

        public async Task<List<Tratamiento>> ConsultarTratamientosPaciente(int idUsuario)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Tratamientos/ConsultarTratamientosPaciente/{idUsuario}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Tratamiento>();

            var respuestaJson = await response.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Tratamiento>>(respuestaJson, opciones) ?? new List<Tratamiento>();
        }

        private static string ExtraerMensajeRespuesta(string respuestaJson)
        {
            if (string.IsNullOrWhiteSpace(respuestaJson)) return string.Empty;
            try
            {
                using var document = JsonDocument.Parse(respuestaJson);
                var root = document.RootElement;
                if (root.TryGetProperty("mensaje", out var mensaje)) return mensaje.GetString() ?? string.Empty;
                if (root.TryGetProperty("Mensaje", out mensaje)) return mensaje.GetString() ?? string.Empty;
            }
            catch { return respuestaJson; }
            return respuestaJson;
        }
    }
}
