using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica.Filters;
using Proyecto_PWA_Clinica.Models;
using Proyecto_PWA_Clinica.Services;

namespace Proyecto_PWA_Clinica.Controllers
{
    public class HomeController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly IUtilitario _utilitario;
        private readonly DashboardService _dashboardService;
        private readonly CitaService _citaService;
        private readonly TratamientoService _tratamientoService;

        public HomeController(
            UsuarioService usuarioService, 
            IUtilitario utilitario,
            DashboardService dashboardService,
            CitaService citaService,
            TratamientoService tratamientoService)
        {
            _usuarioService = usuarioService;
            _utilitario = utilitario;
            _dashboardService = dashboardService;
            _citaService = citaService;
            _tratamientoService = tratamientoService;
        }
        [HttpGet]
        public IActionResult RecuperarAcceso()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarAcceso(RecuperarAccesoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _usuarioService.RecuperarAcceso(model.Correo);

            if (resultado.Item1)
            {
                TempData["MensajeExito"] = "Te hemos enviado un correo con las instrucciones para restablecer tu contraseña.";
                return RedirectToAction("IniciarSesion");
            }

            ModelState.AddModelError("", resultado.Item2);
            return View(model);
        }

        [HttpGet]
        public IActionResult RestablecerAcceso(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("IniciarSesion");

            var model = new RestablecerAccesoViewModel
            {
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerAcceso(RestablecerAccesoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _usuarioService.RestablecerAcceso(
                model.Token,
                model.NuevaContrasena,
                model.ConfirmarContrasena
            );

            if (resultado.Item1)
            {
                TempData["MensajeExito"] = "Tu contraseña se actualizó correctamente.";
                return RedirectToAction("IniciarSesion");
            }

            ModelState.AddModelError("", resultado.Item2);
            return View(model);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> IniciarSesion(Usuario model)
        {
            if (string.IsNullOrWhiteSpace(model.Correo) || string.IsNullOrWhiteSpace(model.Contrasena))
            {
                ModelState.AddModelError("", "Debe ingresar correo y contraseña.");
                return View(model);
            }

            model.Contrasena = _utilitario.Encrypt(model.Contrasena);

            var respuesta = await _usuarioService.IniciarSesion(model);

            if (respuesta.EsCorrecto && respuesta.Usuario != null)
            {
                HttpContext.Session.SetString("NombreUsuario", respuesta.Usuario.Nombre);
                HttpContext.Session.SetString("CorreoUsuario", respuesta.Usuario.Correo);
                HttpContext.Session.SetInt32("IdUsuario", respuesta.Usuario.IdUsuario);
                HttpContext.Session.SetString("Token", respuesta.Token);

                var rolPrincipal = respuesta.Usuario.Roles.FirstOrDefault()?.NombreRol ?? "";
                HttpContext.Session.SetString("RolUsuario", rolPrincipal);

                if (rolPrincipal == "Administrador")
                    return RedirectToAction("DashboardAdmin", "Home");

                if (rolPrincipal == "Paciente")
                    return RedirectToAction("DashboardPaciente", "Home");

                return RedirectToAction("Dashboard", "Home");
            }

            ModelState.AddModelError("", respuesta.Mensaje);
            return View(model);
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Usuario model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Contrasena = _utilitario.Encrypt(model.Contrasena);

            var resultado = await _usuarioService.RegistrarUsuario(model);

            if (resultado.Item1)
            {
                TempData["MensajeExito"] = "Usuario registrado correctamente.";
                return RedirectToAction("IniciarSesion", "Home");
            }

            ModelState.AddModelError("", "No fue posible registrar el usuario.");
            ViewBag.MensajeErrorApi = resultado.Item2;
            return View(model);
        }

        [HttpGet]
        [ValidarSesion]
        public IActionResult Dashboard()
        {
            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            ViewBag.CorreoUsuario = HttpContext.Session.GetString("CorreoUsuario");
            return View();
        }

        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> DashboardPaciente()
        {
            var idUsuarioStr = HttpContext.Session.GetString("IdUsuario");
            if (!int.TryParse(idUsuarioStr, out int idUsuario))
                return RedirectToAction("IniciarSesion");

            var estadisticas = await _dashboardService.ConsultarEstadisticasPaciente(idUsuario);
            var citas = await _citaService.ConsultarCitasPaciente(idUsuario);
            var tratamientos = await _tratamientoService.ConsultarTratamientosPaciente(idUsuario);

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            ViewBag.Citas = citas;
            ViewBag.Tratamientos = tratamientos;

            return View(estadisticas);
        }

        [ValidarSesion]
        public async Task<IActionResult> DashboardAdmin()
        {
            var estadisticas = await _dashboardService.ConsultarEstadisticasAdmin();
            var citas = await _citaService.ConsultarTodasLasCitas();

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            ViewBag.Citas = citas;

            return View(estadisticas);
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}