using Proyecto_PWA_Clinica.Models;
using System.Text.Json;

namespace Proyecto_PWA_Clinica.Services
{
    public class PacienteService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PacienteService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<Paciente>> ConsultarTodosLosPacientes()
        {
            var url = _configuration["Valores:UrlAPI"] + "Pacientes/ConsultarTodosLosPacientes";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return new List<Paciente>();

            var respuestaJson = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            return JsonSerializer.Deserialize<List<Paciente>>(respuestaJson, opciones) ?? new List<Paciente>();
        }

        public async Task<Paciente?> ConsultarDetallePaciente(int idPaciente)
        {
            var url = _configuration["Valores:UrlAPI"] + $"Pacientes/ConsultarDetallePaciente/{idPaciente}";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
                return null;

            var respuestaJson = await response.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            
            return JsonSerializer.Deserialize<Paciente>(respuestaJson, opciones);
        }
    }
}
