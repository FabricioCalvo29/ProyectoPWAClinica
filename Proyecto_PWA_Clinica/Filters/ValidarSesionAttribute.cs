using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Proyecto_PWA_Clinica.Filters
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var nombreUsuario = context.HttpContext.Session.GetString("NombreUsuario");

            if (string.IsNullOrEmpty(nombreUsuario))
            {
                context.Result = new RedirectToActionResult("IniciarSesion", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}