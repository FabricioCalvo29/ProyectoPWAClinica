using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica.Filters;
using Proyecto_PWA_Clinica.Services;
using Proyecto_PWA_Clinica.Models;

namespace Proyecto_PWA_Clinica.Controllers
{
    public class HistorialMedicoController : Controller
    {
        private readonly CitaService _citaService;
        private readonly TratamientoService _tratamientoService;

        public HistorialMedicoController(CitaService citaService, TratamientoService tratamientoService)
        {
            _citaService = citaService;
            _tratamientoService = tratamientoService;
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("IniciarSesion", "Home");

            var citas = await _citaService.ConsultarCitasPaciente(idUsuario.Value);
            var completadas = citas.Where(c => c.EstadoCita == "Completada").ToList();

            var tratamientos = await _tratamientoService.ConsultarTratamientosPaciente(idUsuario.Value);

            ViewBag.Tratamientos = tratamientos;

            return View(completadas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarSesion]
        public async Task<IActionResult> Create(Tratamiento model)
        {
            if (!ModelState.IsValid)
            {
                TempData["MensajeError"] = "Debe completar correctamente la información del tratamiento.";
                return RedirectToAction("MedicoDetails", "Citas", new { id = model.IdCita });
            }

            var (esCorrecto, mensaje) = await _tratamientoService.RegistrarTratamiento(model);

            if (esCorrecto)
                TempData["MensajeExito"] = string.IsNullOrWhiteSpace(mensaje)
                    ? "Tratamiento registrado correctamente."
                    : mensaje;
            else
                TempData["MensajeError"] = string.IsNullOrWhiteSpace(mensaje)
                    ? "No fue posible registrar el tratamiento."
                    : mensaje;

            return RedirectToAction("MedicoDetails", "Citas", new { id = model.IdCita });
        }
    }
}