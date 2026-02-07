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

            return RedirectToAction("Index"); 
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

            TempData["RegistroOK"] = "Registro exitoso. Ahora puedes iniciar sesión.";
            return RedirectToAction(nameof(IniciarSesion));
        }
    }
}
