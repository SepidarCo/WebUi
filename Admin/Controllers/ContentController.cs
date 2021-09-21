using Sepidar.Framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using Sepidar.Admin.Models;

namespace Sepidar.Admin.Controllers
{
    public class ContentController : AdminController
    {
        public IActionResult List(long? filterLanguageId = null)
        {
            if (!filterLanguageId.HasValue)
                filterLanguageId = (long)Language.None;

            var url = $"{BaseUrl}/Content/List?languageId={filterLanguageId.Value}";
            var model = HttpCaller.Get<List<ContentModel>>(url, Request.Headers);

            ViewBag.FilterLanguageId = filterLanguageId.Value;

            return View(model);
        }

        [HttpGet]
        public IActionResult Update(long id)
        {
            var url = $"{BaseUrl}/content/get?id={id}";
            var model = HttpCaller.Get<ContentModel>(url, Request.Headers);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Update(ContentModel model)
        {
            if (!ModelState.IsValid)
                return PartialView(model);

            var url = $"{BaseUrl}/Content/update";
            var formContent = new MultipartFormDataContent();

            if (model.PictureFile != null)
            {
                var stream = model.PictureFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                formContent.Add(streamContent, "PictureFile", model.PictureFile.FileName);
            }

            formContent.Add(new StringContent(model.Id.ToString()), "Id");
            formContent.Add(new StringContent(model.Title), "Title");
            formContent.Add(new StringContent(model.IsActive.ToString()), "IsActive");

            if (model.OrderNumber.IsNotNull())
                formContent.Add(new StringContent(model.OrderNumber.ToString()), "OrderNumber");

            if (model.Description.IsNotNull())
                formContent.Add(new StringContent(model.Description), "Description");

            if (model.LanguageId.IsNotNull())
                formContent.Add(new StringContent(model.LanguageId.ToString()), "LanguageId");

            var updateResult = HttpCaller.Post<dynamic>(url, formContent, Request.Headers);

            if (updateResult.Status.ToString().ToLower() != "success")
            {
                ModelState.AddModelError("", updateResult.Message.ToString());
                return PartialView(model);
            }

            return Json(model.LanguageId);
        }
    }
}
