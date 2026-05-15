
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace appweb2.filtros
{
    public class SessionAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var usuario = context.HttpContext.Session.GetString("usuario");
            if (string.IsNullOrWhiteSpace(usuario))
            {
                context.Result = new RedirectToActionResult("Index", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
