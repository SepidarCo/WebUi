using System.Linq;
using Sepidar.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sepidar.Mvc;

namespace Sepidar.Web
{
    public class NeedsLogin : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Security.IsAuthenticated)
            {
                // if (Web.Security.HasReturnUrl && Web.Security.ReturnUrlIsValid)
                // {
                //     filterContext.Result = new RedirectResult(Web.Security.ReturnUrl);
                //      Web.Security.DeleteReturnUrlCookie();
                // }
                return;
            }
            if (IsPublic(filterContext))
            {
                return;
            }
            //Security.SetReturnUrlCookie();
            filterContext.Result = new RedirectResult(SecurityConfig.LoginPageUrl);
        }

        private bool IsPublic(ActionExecutingContext filterContext)
        {
            var isPublic = filterContext.ActionDescriptor.GetType().GetCustomAttributes(typeof(IsPublic), true).Count() > 0;
            isPublic |= filterContext.Controller.GetType().GetCustomAttributes(typeof(IsPublic), true).Count() > 0;
            return isPublic;
        }
    }
}
