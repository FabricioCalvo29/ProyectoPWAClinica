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

                var rolPrincipalObj =
     respuesta.Usuario.Roles.FirstOrDefault(r => r.NombreRol == "Administrador")
     ?? respuesta.Usuario.Roles.FirstOrDefault(r => r.NombreRol == "Medico")
     ?? respuesta.Usuario.Roles.FirstOrDefault(r => r.NombreRol == "Paciente");

                var rolPrincipal = rolPrincipalObj?.NombreRol ?? "";
                var idRolPrincipal = rolPrincipalObj?.IdRol.ToString() ?? "";

                HttpContext.Session.SetString("RolUsuario", rolPrincipal);
                HttpContext.Session.SetString("IdRolPrincipal", idRolPrincipal);

                if (rolPrincipal == "Administrador")
                    return RedirectToAction("DashboardAdmin", "Home");

                if (rolPrincipal == "Medico")
                    return RedirectToAction("DashboardMedico", "Home");

                if (rolPrincipal == "Paciente")
                    return RedirectToAction("DashboardPaciente", "Home");

                return RedirectToAction("Dashboard", "Home");
            }

            ModelState.AddModelError("", respuesta.Mensaje);
            return View(model);
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
        [ValidarSesion]
        public async Task<IActionResult> DashboardPaciente()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
                return RedirectToAction("IniciarSesion");

            var estadisticas = await _dashboardService.ConsultarEstadisticasPaciente(idUsuario.Value);
            var citas = await _citaService.ConsultarCitasPaciente(idUsuario.Value);
            var tratamientos = await _tratamientoService.ConsultarTratamientosPaciente(idUsuario.Value);

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            ViewBag.Citas = citas;
            ViewBag.Tratamientos = tratamientos;

            return View(estadisticas);
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
        public async Task<IActionResult> DashboardAdmin()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
                return RedirectToAction("IniciarSesion");

            var estadisticas = await _dashboardService.ConsultarEstadisticasAdmin();
            var citas = await _citaService.ConsultarTodasLasCitas();

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            ViewBag.Citas = citas;

            return View(estadisticas);
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
                return View(model);

            model.Contrasena = _utilitario.Encrypt(model.Contrasena);

            var resultado = await _usuarioService.RegistrarUsuario(model);

            if (resultado.Item1)
            {
                TempData["MensajeExito"] = "Usuario registrado correctamente.";
                return RedirectToAction("IniciarSesion", "Home");
            }

            // Traducir errores técnicos de BD a mensajes amigables para el usuario
            string mensajeError = resultado.Item2 ?? "Error desconocido.";
            string mensajeAmigable;

            if (mensajeError.Contains("FOREIGN KEY") || mensajeError.Contains("FK_"))
                mensajeAmigable = "Error de configuración interna. Contacte al administrador del sistema.";
            else if (mensajeError.Contains("UNIQUE") || mensajeError.Contains("duplicate key") || mensajeError.Contains("Duplicate"))
                mensajeAmigable = "El correo electrónico o la cédula ingresada ya está registrada. Intente con otro.";
            else if (mensajeError.Contains("NULL") || mensajeError.Contains("cannot be null"))
                mensajeAmigable = "Faltan datos obligatorios. Verifique todos los campos del formulario.";
            else if (mensajeError.Contains("timeout") || mensajeError.Contains("connection"))
                mensajeAmigable = "No fue posible conectar con el servidor. Intente nuevamente en unos momentos.";
            else
                mensajeAmigable = "No fue posible completar el registro. Por favor intente nuevamente.";

            ModelState.AddModelError("", mensajeAmigable);
            ViewBag.MensajeErrorApi = mensajeAmigable;
            return View(model);
        }

        [HttpGet]
        public IActionResult CerrarSesion()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        [ValidarSesion]
        public async Task<IActionResult> DashboardMedico()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
                return RedirectToAction("IniciarSesion");

            var estadisticas = await _dashboardService.ConsultarEstadisticasMedico(idUsuario.Value);

            var citas = await _citaService.ConsultarCitasMedico(idUsuario.Value);
            citas = citas
                .Where(c => c.EstadoCita == "Pendiente")
                .OrderBy(c => c.FechaHora)
                .ToList();

            ViewBag.NombreUsuario = HttpContext.Session.GetString("NombreUsuario");
            ViewBag.Citas = citas;

            return View(estadisticas);
        }
    }
}