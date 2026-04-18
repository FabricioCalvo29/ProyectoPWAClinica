using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Proyecto_PWA_Clinica.Filters;
using Proyecto_PWA_Clinica.Models;
using Proyecto_PWA_Clinica.Services;

namespace Proyecto_PWA_Clinica.Controllers
{
    public class MedicosController : Controller
    {
        private readonly MedicoService _medicoService;
        private readonly IUtilitario _utilitario;

        public MedicosController(MedicoService medicoService, IUtilitario utilitario)
        {
            _medicoService = medicoService;
            _utilitario = utilitario;
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Index()
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1")
                return RedirectToAction("DashboardPaciente", "Home");

            var medicos = await _medicoService.ConsultarTodosLosMedicos();
            return View(medicos);
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Create()
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1")
                return RedirectToAction("DashboardPaciente", "Home");

            await CargarEspecialidades();
            return View(new RegistrarMedicoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarSesion]
        public async Task<IActionResult> Create(RegistrarMedicoViewModel model)
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1")
                return RedirectToAction("DashboardPaciente", "Home");

            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(model);
            }

            model.Contrasenna = _utilitario.Encrypt(model.Contrasenna);

            var resultado = await _medicoService.RegistrarMedico(model);

            if (resultado.Item1)
            {
                TempData["MensajeExito"] = resultado.Item2;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", resultado.Item2);
            await CargarEspecialidades();
            return View(model);
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> Edit(int id)
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1")
                return RedirectToAction("DashboardPaciente", "Home");

            var medico = await _medicoService.ConsultarDetalleMedico(id);
            if (medico == null)
            {
                TempData["MensajeError"] = "Médico no encontrado.";
                return RedirectToAction("Index");
            }

            await CargarEspecialidades();
            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarSesion]
        public async Task<IActionResult> Edit(RegistrarMedicoViewModel model)
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1")
                return RedirectToAction("DashboardPaciente", "Home");

            ModelState.Remove(nameof(model.Contrasenna));
            ModelState.Remove(nameof(model.ConfirmarContrasenna));

            if (!ModelState.IsValid)
            {
                await CargarEspecialidades();
                return View(model);
            }

            var resultado = await _medicoService.ActualizarMedico(model);

            if (resultado.Item1)
            {
                TempData["MensajeExito"] = resultado.Item2;
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", resultado.Item2);
            await CargarEspecialidades();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarSesion]
        public async Task<IActionResult> Inactivar(int idMedico)
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1")
                return RedirectToAction("DashboardPaciente", "Home");

            var resultado = await _medicoService.CambiarEstadoMedico(idMedico, false);

            if (resultado.Item1)
                TempData["MensajeExito"] = resultado.Item2;
            else
                TempData["MensajeError"] = resultado.Item2;

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidarSesion]
        public async Task<IActionResult> Activar(int idMedico)
        {
            var idRolStr = HttpContext.Session.GetString("IdRolPrincipal");
            if (idRolStr != "1")
                return RedirectToAction("DashboardPaciente", "Home");

            var resultado = await _medicoService.CambiarEstadoMedico(idMedico, true);

            if (resultado.Item1)
                TempData["MensajeExito"] = resultado.Item2;
            else
                TempData["MensajeError"] = resultado.Item2;

            return RedirectToAction("Index");
        }

        private async Task CargarEspecialidades()
        {
            var especialidades = await _medicoService.ConsultarEspecialidadesActivas();

            ViewBag.Especialidades = especialidades
                .Select(x => new SelectListItem
                {
                    Value = x.IdEspecialidad.ToString(),
                    Text = x.Nombre
                })
                .ToList();
        }
    }
}