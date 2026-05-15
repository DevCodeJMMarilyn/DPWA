using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace appweb2.filtros
{
    public class AdminAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.Session.GetString("usuario");
            if (string.IsNullOrWhiteSpace(usuario))
            {
                context.Result = new RedirectToActionResult("Index", "Account", null);
                return;
            }

            var rol = context.HttpContext.Session.GetString("rol");
            if (!string.Equals(rol, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToActionResult("Index", "VideoJuegos", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
