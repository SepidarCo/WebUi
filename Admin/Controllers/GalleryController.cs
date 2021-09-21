using Microsoft.AspNetCore.Mvc;
using Sepidar.Admin.Models;
using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System.Net.Http;

namespace Sepidar.Admin.Controllers
{
    public class GalleryController : AdminController
    {
        [HttpGet]
        public ActionResult List(long? filterLanguageId = null, int pageNumber = 1)
        {
            string url;

            if (!Sepidar.Admin.Config.HasLanguageInGallery)
            {
                filterLanguageId = (long)Language.None;
            }
            else
            {
                filterLanguageId = (long)Language.English;
            }

            if (filterLanguageId.IsNotNull())
            {
                url = $"{BaseUrl}/Gallery/List?languageId={filterLanguageId.Value}&PageNumber={pageNumber}";
            }
            else
            {
                url = $"{BaseUrl}/Gallery/List?PageNumber={pageNumber}";
            }

            var model = HttpCaller.Get<ListResult<GalleryModel>>(url, Request.Headers);

            ViewBag.FilterLanguageId = filterLanguageId.Value;

            return View(model);
        }


        [HttpGet]
        public IActionResult Create(long? filterLanguageId)
        {
            if (!filterLanguageId.HasValue)
                filterLanguageId = (long)Language.English;

            var model = new GalleryModel
            {
                IsActive = true,
                LanguageId = filterLanguageId.Value
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(GalleryModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var url = $"{BaseUrl}/Gallery/Create";

            var formContent = new MultipartFormDataContent();
            var stream = model.PictureFile.OpenReadStream();
            var streamContent = new StreamContent(stream);
            formContent.Add(streamContent, "PictureFile", model.PictureFile.FileName);

            if (model.LanguageId.IsNotNull())
                formContent.Add(new StringContent(model.LanguageId.ToString()), "LanguageId");

            if (model.Title.IsNotNull())
                formContent.Add(new StringContent(model.Title), "Title");

            if (model.Alt.IsNotNull())
                formContent.Add(new StringContent(model.Alt), "Alt");

            if (model.OrderNumber.IsNotNull())
                formContent.Add(new StringContent(model.OrderNumber.ToString()), "OrderNumber");

            if (model.IsActive.IsNotNull())
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
            var url = $"{BaseUrl}/gallery/get?id={id}";
            var model = HttpCaller.Get<GalleryModel>(url, Request.Headers);
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(GalleryModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var formContent = new MultipartFormDataContent();

            formContent.Add(new StringContent(model.Id.ToString()), "Id");

            if (model.PictureFile != null)
            {
                var stream = model.PictureFile.OpenReadStream();
                var streamContent = new StreamContent(stream);
                formContent.Add(streamContent, "PictureFile", model.PictureFile.FileName);
            }

            if (model.LanguageId.IsNotNull())
                formContent.Add(new StringContent(model.LanguageId.ToString()), "LanguageId");

            if (model.Title.IsNotNull())
                formContent.Add(new StringContent(model.Title), "Title");

            if (model.Alt.IsNotNull())
                formContent.Add(new StringContent(model.Alt), "Alt");

            if (model.OrderNumber.IsNotNull())
                formContent.Add(new StringContent(model.OrderNumber.ToString()), "OrderNumber");

            if (model.IsActive.IsNotNull())
                formContent.Add(new StringContent(model.IsActive.ToString()), "IsActive");

            var url = $"{BaseUrl}/gallery/update";
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
        public IActionResult Delete(DeleteModel model)
        {
            var url = $"{BaseUrl}/Gallery/Delete?Id={model.ModelId}";
            HttpCaller.Delete(url, Request.Headers);
            return PartialView(model);
        }
    }
}
