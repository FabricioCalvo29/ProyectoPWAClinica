using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica.Filters;
using Proyecto_PWA_Clinica.Models;
using Proyecto_PWA_Clinica.Services;

namespace Proyecto_PWA_Clinica.Controllers
{
    public class CitasController : Controller
    {
        private readonly CitaService _citaService;

        public CitasController(CitaService citaService)
        {
            _citaService = citaService;
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Index()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("IniciarSesion", "Home");

            var citas = await _citaService.ConsultarCitasPaciente(idUsuario.Value);
            return View(citas);
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Create()
        {
            await CargarMedicos();

            var fechaBase = DateTime.Now.AddDays(1);
            var model = new Cita
            {
                FechaHora = new DateTime(
                    fechaBase.Year,
                    fechaBase.Month,
                    fechaBase.Day,
                    fechaBase.Hour,
                    fechaBase.Minute,
                    0
                )
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarSesion]
        public async Task<IActionResult> Create(Cita model)
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("IniciarSesion", "Home");

            if (!ModelState.IsValid)
            {
                await CargarMedicos();
                return View(model);
            }

            model.FechaHora = new DateTime(
                model.FechaHora.Year,
                model.FechaHora.Month,
                model.FechaHora.Day,
                model.FechaHora.Hour,
                model.FechaHora.Minute,
                0
            );

            var resultado = await _citaService.RegistrarCitaPaciente(idUsuario.Value, model);

            if (resultado.Item1)
            {
                TempData["MensajeExito"] = resultado.Item2;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", resultado.Item2);
            await CargarMedicos();
            return View(model);
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Details(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("IniciarSesion", "Home");

            var cita = await _citaService.ConsultarDetalleCitaPaciente(idUsuario.Value, id);

            if (cita == null)
            {
                TempData["MensajeError"] = "No fue posible encontrar la cita solicitada.";
                return RedirectToAction("Index");
            }

            return View(cita);
        }

        private async Task CargarMedicos()
        {
            var resultado = await _citaService.ConsultarMedicosActivosParaCita();
            var medicos = resultado.Item1;

            ViewBag.Medicos = medicos;
            ViewBag.ErrorMedicos = resultado.Item2;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarSesion]
        public async Task<IActionResult> Cancelar(int id)
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
                return RedirectToAction("IniciarSesion", "Home");

            var resultado = await _citaService.CancelarCitaPaciente(idUsuario.Value, id);

            if (resultado.Item1)
                TempData["MensajeExito"] = resultado.Item2;
            else
                TempData["MensajeError"] = resultado.Item2;

            return RedirectToAction("Index");
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> AdminIndex()
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1" && idRolStr != "2")
                return RedirectToAction("DashboardPaciente", "Home");

            var citas = await _citaService.ConsultarTodasLasCitas();
            return View(citas);
        }
    }
}
