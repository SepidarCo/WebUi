using Microsoft.AspNetCore.Mvc.Filters;
using Sepidar.Site.Models;
using Sepidar.Web.Controllers;

namespace Sepidar.Site.Controllers
{
    public class BaseController: GeneralController
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string urlSettings = $"{BaseUrl}/Module/Settings";
            var modelSettings = HttpCaller.Get<SettingModel>(urlSettings);
            ViewBag.Settings = modelSettings;
        }
    }
}
