using Microsoft.AspNetCore.Mvc;
using Sepidar.Admin.Models;
using System.Collections.Generic;

namespace Sepidar.Admin.Controllers
{
    public class SettingController : AdminController
    {
        [HttpGet]
        public IActionResult List()
        {
            var url = $"{BaseUrl}/setting/list";
            var model = HttpCaller.Get<List<SettingModel>>(url, Request.Headers);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpGet]
        public IActionResult Update(long id)
        {
            var url = $"{BaseUrl}/setting/get?id={id}";
            var model = HttpCaller.Get<SettingModel>(url, Request.Headers);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Update(SettingModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var url = $"{BaseUrl}/Setting/update";
            var updateResult = HttpCaller.Post<dynamic>(url, model, Request.Headers);

            if (updateResult.Status.ToString().ToLower() != "success")
            {
                ModelState.AddModelError("", updateResult.Message.ToString());
                return PartialView(model);
            }

            return RedirectToAction("List", "setting");
        }
    }
}
