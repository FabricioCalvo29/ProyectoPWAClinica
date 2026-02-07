using Microsoft.AspNetCore.Mvc;
using Proyecto_PWA_Clinica.Models;

namespace Proyecto_PWA_Clinica.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // LOGIN
        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IniciarSesion(Usuario modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            // TODO: aquí validarías credenciales contra BD/servicio.
            // Por ahora solo ejemplo:
            // if (!AuthService.Login(modelo.Correo, modelo.Contrasenna)) { ModelState.AddModelError("", "Credenciales inválidas"); return View(modelo); }

            return RedirectToAction("Index"); // o Dashboard cuando exista
        }

        // REGISTRO
        [HttpGet]
        public IActionResult Registro()
        {
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Usuario modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            // TODO: aquí guardarías el usuario en BD.
            // Importante: Hashear contraseña antes de guardar.
            // Ejemplo: UserService.Create(modelo);

            // Al terminar, mandas al login:
            TempData["RegistroOK"] = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction(nameof(IniciarSesion));
        }
    }
}
