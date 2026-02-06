using System.Diagnostics;
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

        [HttpGet]
        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public IActionResult IniciarSesion(Usuario modelo)
        {
            return View();
        }

    }
}
