using Microsoft.AspNetCore.Mvc;
using Sepidar.Admin.Models;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System.Net.Http;

namespace Sepidar.Admin.Controllers
{
    public class SliderController : AdminController
    {
        [HttpGet]
        public ActionResult List(long? filterLanguageId = null, int pageNumber = 1)
        {
            string url;

            if (!Sepidar.Admin.Config.HasLanguageInSlider)
            {
                filterLanguageId = (long)Language.None;
            }
            else
            {
                filterLanguageId = (long)Language.English;
            }

            if (filterLanguageId.IsNotNull())
            {
                url = $"{BaseUrl}/Slider/List?languageId={filterLanguageId.Value}&PageNumber={pageNumber}";
            }
            else
            {
                url = $"{BaseUrl}/Slider/List?PageNumber={pageNumber}";
            }

            var model = HttpCaller.Get<ListResult<SliderModel>>(url, Request.Headers);

            ViewBag.FilterLanguageId = filterLanguageId.Value;

            return View(model);
        }

        [HttpGet]
        public IActionResult Create(long? filterLanguageId)
        {
            if (!filterLanguageId.HasValue)
                filterLanguageId = (long)Language.English;

            var model = new SliderModel
            {
                IsActive = true,
                LanguageId = filterLanguageId.Value
            };

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Create(SliderModel model)
        {
            if (model.PictureFile == null)
                ModelState.AddModelError("PictureFile", "Please select picture file");

            if (!ModelState.IsValid)
                return PartialView(model);

            var url = $"{BaseUrl}/slider/create";

            var formContent = new MultipartFormDataContent();
            var stream = model.PictureFile.OpenReadStream();
            var streamContent = new StreamContent(stream);
            formContent.Add(streamContent, "PictureFile", model.PictureFile.FileName);

            if (model.LanguageId.IsNotNull())
                formContent.Add(new StringContent(model.LanguageId.ToString()), "LanguageId");

            if (model.Title.IsNotNull())
                formContent.Add(new StringContent(model.Title.ToString()), "Title");

            if (model.Description.IsNotNull())
                formContent.Add(new StringContent(model.Description.ToString()), "Description");

            if (model.OrderNumber.IsNotNull())
                formContent.Add(new StringContent(model.OrderNumber.ToString()), "OrderNumber");

            if (model.LinkUrl.IsNotNull())
                formContent.Add(new StringContent(model.LinkUrl.ToString()), "LinkUrl");

            formContent.Add(new StringContent(model.IsActive.ToString()), "IsActive");

            var result = HttpCaller.Post<dynamic>(url, formContent, Request.Headers);

            if (result.Status.ToString().ToLower() != "success")
            {
                ModelState.AddModelError("", result.Message.ToString());
                return PartialView(model);
            }

            return Json(model.LanguageId);
        }

        [HttpGet]
        public IActionResult Update(long id)
        {
            var url = $"{BaseUrl}/slider/get?id={id}";
            var model = HttpCaller.Get<SliderModel>(url, Request.Headers);
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Update(SliderModel model)
        {
            var url = $"{BaseUrl}/slider/update";

            var formContent = new MultipartFormDataContent();

            if (model.PictureFile != null)
            {
                var stream = model.PictureFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                formContent.Add(streamContent, "PictureFile", model.PictureFile.FileName);
            }

            if (model.LanguageId.IsNotNull())
                formContent.Add(new StringContent(model.LanguageId.ToString()), "LanguageId");

            if (model.Title.IsNotNull())
                formContent.Add(new StringContent(model.Title.ToString()), "Title");

            if (model.Description.IsNotNull())
                formContent.Add(new StringContent(model.Description.ToString()), "Description");

            if (model.LinkUrl.IsNotNull())
                formContent.Add(new StringContent(model.LinkUrl.ToString()), "LinkUrl");

            formContent.Add(new StringContent(model.Id.ToString()), "Id");
            formContent.Add(new StringContent(model.OrderNumber.ToString()), "OrderNumber");
            formContent.Add(new StringContent(model.IsActive.ToString()), "IsActive");

            var result = HttpCaller.Post<dynamic>(url, formContent, Request.Headers);

            if (result.Status.ToString().ToLower() != "success")
            {
                ModelState.AddModelError("", result.Message.ToString());
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
            var url = $"{BaseUrl}/slider/delete?id={model.ModelId}";
            HttpCaller.Delete(url, Request.Headers);
            return PartialView(model);
        }
    }
}