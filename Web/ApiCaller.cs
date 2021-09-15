using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Sepidar.Framework.Extensions;
using Sepidar.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sepidar.Mvc;

namespace Sepidar.Web
{
    public class ApiCaller : IApiCaller
    {
        private IConfiguration configuration;
        private IHttpContextAccessor httpContextAccessor;

        public ApiCaller(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public T Get<T>(string url)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies
            };

            using var httpClient = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

            AddingHeadersToHttpClientRequest(httpContextAccessor.HttpContext.Request.Headers, httpClient);

            var response = httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
                return default;

            var responseBody = response.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<ApiResponseModel>(responseBody);
            if (result.Status != "success")
                return default;

            string resultStr = result.Result.ToString();

            return JsonConvert.DeserializeObject<T>(resultStr);
        }

        public T Post<T>(string uri, HttpContent content, bool getCookie = false)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies
            };

            using var httpClient = new HttpClient(handler);
            var requestUri = new Uri(uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AddingHeadersToHttpClientRequest(httpContextAccessor.HttpContext.Request.Headers, httpClient);

            if (content != null)
                request.Content = content;

            var response = httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
                return default;

            var result = response.Content.ReadAsStringAsync().Result.JsonDeserialize<T>();

            if (getCookie)
            {
                AppendCookieToHttpContext(configuration["Security:AuthenticationCookieName"], cookies, requestUri);
            }

            return result;
        }

        public T Post<T>(string uri, object body, bool getCookie = false)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies
            };

            using var httpClient = new HttpClient(handler);
            var requestUri = new Uri(uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AddingHeadersToHttpClientRequest(httpContextAccessor.HttpContext.Request.Headers, httpClient);

            if (body != null)
            {
                var jsonSerialize = body.JsonSerialize();
                request.Content = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
            }

            var response = httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
                return default;

            var result = response.Content.ReadAsStringAsync().Result.JsonDeserialize<T>();

            if (getCookie)
            {
                AppendCookieToHttpContext(configuration["Security:AuthenticationCookieName"], cookies, requestUri);
            }

            return result;
        }

        public void Post(string uri, object body = null)
        {
            Post<object>(uri, body);
        }

        private void AddingHeadersToHttpClientRequest(IHeaderDictionary requestHeaders, HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", requestHeaders["User-Agent"].ToString());
            httpClient.DefaultRequestHeaders.Add("Origin", requestHeaders["Origin"].ToString());
            httpClient.DefaultRequestHeaders.Add("Cookie", requestHeaders["Cookie"].ToString());

            if (requestHeaders.ContainsKey("Referer"))
                httpClient.DefaultRequestHeaders.Add("Referer", requestHeaders["Referer"].ToString());
        }

        private void AppendCookieToHttpContext(string cookieName, CookieContainer cookies, Uri requestUri)
        {
            var cookie = GetCookie(cookieName, cookies, requestUri);
            if (cookie.IsNotNull())
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = cookie.HttpOnly,
                    Expires = cookie.Expires,
                    Domain = cookie.Domain
                };
                AppHttpContext.Current.Response.Cookies.Append(cookie.Name, cookie.Value, cookieOptions);
            }
        }

        private Cookie GetCookie(string cookieName, CookieContainer cookies, Uri requestUri)
        {
            var cookie = cookies.GetCookies(requestUri).FirstOrDefault(x => x.Name == cookieName);
            if (cookie.IsNotNull())
            {
                cookie.Value = WebUtility.UrlDecode(cookie.Value);
            }
            return cookie;
        }

        public void RemoveCookie()
        {
            var authenticationCookieName = SecurityConfig.AuthenticationCookieName;
            var cookieOptions = Mvc.Security.CreateCookieOptions(DateTime.Now.AddDays(-1));
            AppHttpContext.Current.Response.Cookies.Append(authenticationCookieName, "", cookieOptions);
        }

    }
}
