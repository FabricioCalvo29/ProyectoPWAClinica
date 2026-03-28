using Proyecto_PWA_Clinica.Models;
using System.Text.Json;

namespace Proyecto_PWA_Clinica.Services
{
    public class DashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DashboardService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<EstadisticasAdmin> ConsultarEstadisticasAdmin()
        {
            var url = _configuration["Valores:UrlAPI"] + "Dashboard/EstadisticasAdmin";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new EstadisticasAdmin();

            var respuestaJson = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            return JsonSerializer.Deserialize<EstadisticasAdmin>(respuestaJson, opciones) ?? new EstadisticasAdmin();
        }

        public async Task<EstadisticasPaciente> ConsultarEstadisticasPaciente(int idUsuario)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Dashboard/EstadisticasPaciente/{idUsuario}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new EstadisticasPaciente();

            var respuestaJson = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            return JsonSerializer.Deserialize<EstadisticasPaciente>(respuestaJson, opciones) ?? new EstadisticasPaciente();
        }
    }
}
