using Microsoft.AspNetCore.Mvc;
using Sepidar.Admin.Models;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;

namespace Sepidar.Admin.Controllers
{
    public class CalendarController : AdminController
    {
        [HttpGet]
        public ActionResult List(long? filterLanguageId = null, int pageNumber = 1)
        {
            string url;

            if (!Config.HasLanguageInCalendar)
            {
                filterLanguageId = (long)Language.None;
            }
            else
            {
                filterLanguageId = (long)Language.English;
            }

            if (filterLanguageId.IsNotNull())
            {
                url = $"{BaseUrl}/Calendar/list?languageId={filterLanguageId.Value}&PageNumber={pageNumber}";
            }
            else
            {
                url = $"{BaseUrl}/Calendar/list?PageNumber={pageNumber}";
            }

            var model = HttpCaller.Get<ListResult<CalendarModel>>(url, Request.Headers);

            ViewBag.FilterLanguageId = filterLanguageId.Value;

            return View(model);
        }

        [HttpGet]
        public IActionResult Create(long? filterLanguageId)
        {
            if (!filterLanguageId.HasValue)
                filterLanguageId = (long)Language.English;

            var calendarModel = new CalendarModel
            {
                IsActive = true,
                LanguageId = filterLanguageId.Value
            };

            return PartialView(calendarModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CalendarModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var url = $"{BaseUrl}/Calendar/create";
            var createResult = HttpCaller.Post<dynamic>(url, model, Request.Headers);

            if (createResult.Status.ToString().ToLower() != "success")
            {
                ModelState.AddModelError("", createResult.Message.ToString());
                return PartialView(model);
            }

            return Json(model.LanguageId);
        }

        [HttpGet]
        public IActionResult Update(long id)
        {
            var url = $"{BaseUrl}/Calendar/get?id={id}";
            var model = HttpCaller.Get<CalendarModel>(url, Request.Headers);
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(CalendarModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var url = $"{BaseUrl}/Calendar/update";
            var updateResult = HttpCaller.Post<dynamic>(url, model, Request.Headers);

            if (updateResult.Status.ToString().ToLower() != "success")
            {
                ModelState.AddModelError("", updateResult.Message.ToString());
                return PartialView(model);
            }

            return Json(model.LanguageId);
        }

        [HttpGet]
        public IActionResult Delete(long id)
        {
            var model = new DeleteModel
            {
                ModelId = id
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(DeleteModel model)
        {
            var url = $"{BaseUrl}/Calendar/delete?id={model.ModelId}";
            HttpCaller.Delete(url, Request.Headers);

            return PartialView(model);
        }
    }
}
