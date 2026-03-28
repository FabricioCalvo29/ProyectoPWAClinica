using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica.Filters;
using Proyecto_PWA_Clinica.Services;

namespace Proyecto_PWA_Clinica.Controllers
{
    public class PacientesController : Controller
    {
        private readonly PacienteService _pacienteService;

        public PacientesController(PacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Index()
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1" && idRolStr != "2") // Solo Admin o Personal
                return RedirectToAction("DashboardPaciente", "Home");

            var pacientes = await _pacienteService.ConsultarTodosLosPacientes();
            return View(pacientes);
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Details(int id)
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1" && idRolStr != "2")
                return RedirectToAction("DashboardPaciente", "Home");

            var paciente = await _pacienteService.ConsultarDetallePaciente(id);
            if (paciente == null)
            {
                TempData["MensajeError"] = "Paciente no encontrado.";
                return RedirectToAction("Index");
            }

            return View(paciente);
        }
    }
}
