using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sepidar.Web.Controllers
{
    public class GeneralController: Controller
    {
        private IConfiguration configuration;
        private IHttpCaller httpCaller;
        private IApiCaller apiCaller;

        public IConfiguration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
                }
                return configuration;
            }
        }

        public IHttpCaller HttpCaller
        {
            get
            {
                if (httpCaller == null)
                {
                    httpCaller = HttpContext.RequestServices.GetRequiredService<IHttpCaller>();
                }
                return httpCaller;
            }
        }

        public IApiCaller ApiCaller
        {
            get
            {
                if (apiCaller == null)
                {
                    apiCaller = HttpContext.RequestServices.GetRequiredService<IApiCaller>();
                }
                return apiCaller;
            }
        }

        public string BaseUrl
        {
            get
            {
                return Configuration["Settings:ServiceUrl"];
            }
        }
    }
}
