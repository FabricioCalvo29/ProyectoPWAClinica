using Proyecto_PWA_Clinica.Models;
using System.Text;
using System.Text.Json;

namespace Proyecto_PWA_Clinica.Services
{
    public class CitaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CitaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<(List<MedicoDisponible>, string)> ConsultarMedicosActivosParaCita()
        {
            try
            {
                var url = _configuration["Valores:UrlAPI"] + "Citas/ConsultarMedicosActivosParaCita";
                var response = await _httpClient.GetAsync(url);
                var respuestaJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var mensaje = ExtraerMensajeRespuesta(respuestaJson);
                    if (string.IsNullOrWhiteSpace(mensaje))
                        mensaje = "No fue posible consultar los medicos disponibles.";

                    return (new List<MedicoDisponible>(), mensaje);
                }

                var opciones = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var medicos = JsonSerializer.Deserialize<List<MedicoDisponible>>(respuestaJson, opciones) ?? new List<MedicoDisponible>();
                return (medicos, string.Empty);
            }
            catch (Exception ex)
            {
                return (new List<MedicoDisponible>(), ex.Message);
            }
        }

        public async Task<(bool, string)> RegistrarCitaPaciente(int idUsuario, Cita model)
        {
            var datos = new
            {
                IdUsuario = idUsuario,
                model.IdMedico,
                model.FechaHora,
                model.Motivo
            };

            var url = _configuration["Valores:UrlAPI"] + "Citas/RegistrarCitaPaciente";

            var json = JsonSerializer.Serialize(datos);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var respuestaJson = await response.Content.ReadAsStringAsync();

            var mensaje = ExtraerMensajeRespuesta(respuestaJson);
            return (response.IsSuccessStatusCode, mensaje);
        }

        public async Task<List<Cita>> ConsultarCitasPaciente(int idUsuario)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Citas/ConsultarCitasPaciente/{idUsuario}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Cita>();

            var respuestaJson = await response.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Cita>>(respuestaJson, opciones) ?? new List<Cita>();
        }

        public async Task<Cita?> ConsultarDetalleCitaPaciente(int idUsuario, int idCita)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Citas/ConsultarDetalleCitaPaciente/{idUsuario}/{idCita}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var respuestaJson = await response.Content.ReadAsStringAsync();

            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<Cita>(respuestaJson, opciones);
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

        public async Task<(bool, string)> CancelarCitaPaciente(int idUsuario, int idCita)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Citas/CancelarCitaPaciente/{idUsuario}/{idCita}";
            var response = await _httpClient.PutAsync(url, null);
            var respuestaJson = await response.Content.ReadAsStringAsync();
            var mensaje = ExtraerMensajeRespuesta(respuestaJson);
            return (response.IsSuccessStatusCode, mensaje);
        }

        public async Task<(bool, string)> CompletarCita(int idCita)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Citas/CompletarCita/{idCita}";
            var response = await _httpClient.PutAsync(url, null);
            var respuestaJson = await response.Content.ReadAsStringAsync();
            var mensaje = ExtraerMensajeRespuesta(respuestaJson);
            return (response.IsSuccessStatusCode, mensaje);
        }

        public async Task<List<Cita>> ConsultarTodasLasCitas()
        {
            var url = _configuration["Valores:UrlAPI"] + "Citas/ConsultarTodasLasCitas";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Cita>();

            var respuestaJson = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<Cita>>(respuestaJson, opciones) ?? new List<Cita>();
        }
        public async Task<List<Cita>> ConsultarCitasMedico(int idUsuario)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Citas/ConsultarCitasMedico/{idUsuario}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<Cita>();

            var respuestaJson = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return JsonSerializer.Deserialize<List<Cita>>(respuestaJson, opciones) ?? new List<Cita>();
        }

        public async Task<Cita?> ConsultarDetalleCitaMedico(int idUsuario, int idCita)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Citas/ConsultarDetalleCitaMedico/{idUsuario}/{idCita}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null;

            var respuestaJson = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            return JsonSerializer.Deserialize<Cita>(respuestaJson, opciones);
        }
    }
}
