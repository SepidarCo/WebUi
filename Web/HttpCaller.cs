using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Sepidar.Framework.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sepidar.Mvc;

namespace Sepidar.Web
{
    public class HttpCaller : IHttpCaller
    {
        private IConfiguration configuration;

        public HttpCaller(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private void AddingHeadersToHttpClientRequest(IHeaderDictionary requestHeaders, HttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", requestHeaders["User-Agent"].ToString());
            httpClient.DefaultRequestHeaders.Add("Origin", requestHeaders["Origin"].ToString());

            if (requestHeaders.ContainsKey("Referer"))
                httpClient.DefaultRequestHeaders.Add("Referer", requestHeaders["Referer"].ToString());
        }

        private void AppendCookieToHttpContext(string cookieName, CookieContainer cookies, Uri requestUri)
        {
            var cookie = GetCookie(cookieName, cookies, requestUri);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = cookie.HttpOnly,
                Expires = cookie.Expires,
                Domain = cookie.Domain
            };
            AppHttpContext.Current.Response.Cookies.Append(cookie.Name, cookie.Value, cookieOptions);
        }

        private CookieContainer GetCookieContainer()
        {
            var cookieName = configuration["Security:AuthenticationCookieName"];
            var cookieDomain = configuration["Security:AuthenticationCookieDomain"];

            var request = AppHttpContext.Current.Request;
            var response = AppHttpContext.Current.Response;

            var cookieContainer = new CookieContainer();

            var cookieValue = request.Cookies[cookieName].Replace("%2F", "/");

            var cookie = new Cookie(cookieName, cookieValue, response.HttpContext.Request.PathBase)
            {
                Domain = cookieDomain
            };

            cookieContainer.Add(cookie);

            return cookieContainer;
        }

        private Cookie GetCookie(string cookieName, CookieContainer cookies, Uri requestUri)
        {
            var cookie = cookies.GetCookies(requestUri).FirstOrDefault(x => x.Name == cookieName);
            cookie.Value = cookie.Value.Replace("%2F", "/");
            return cookie; 
        }

        public bool Delete(string uri, IHeaderDictionary requestHeaders)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = GetCookieContainer()
            };

            using var httpClient = new HttpClient(handler);

            AddingHeadersToHttpClientRequest(requestHeaders, httpClient);

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));
            var response = httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
                return false;

            var result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);

            return true;
        }

        public T Get<T>(string url)
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

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

        public T Get<T>(string url, IHeaderDictionary requestHeaders)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = GetCookieContainer()
            };

            using var httpClient = new HttpClient(handler);
            AddingHeadersToHttpClientRequest(requestHeaders, httpClient);

            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

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

        public T Post<T>(string uri, object body)
        {
            using var httpClient = new HttpClient();
            var requestUri = new Uri(uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            if (body != null)
            {
                var jsonSerialize = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
            }
            var response = httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
                return default;

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(responseBody);

            return result;
        }

        public T Post<T>(string uri, object body, IHeaderDictionary requestHeaders)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = GetCookieContainer()
            };

            using var httpClient = new HttpClient(handler);
            AddingHeadersToHttpClientRequest(requestHeaders, httpClient);

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));

            if (body != null)
            {
                var jsonBody = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            }

            var response = httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
                return default;

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(responseBody);

            return result;
        }

        public T Post<T>(string url, HttpContent content, IHeaderDictionary requestHeaders)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = GetCookieContainer()
            };

            using var httpClient = new HttpClient(handler);
            AddingHeadersToHttpClientRequest(requestHeaders, httpClient);

            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(url));

            if (content != null)
                request.Content = content;

            var response = httpClient.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
                return default;

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<T>(responseBody);

            return result;
        }

        public T PostAndGetCookie<T>(string uri, object body, string cookieName, IHeaderDictionary requestHeaders)
        {
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies
            };

            using var httpClient = new HttpClient(handler);
            var requestUri = new Uri(uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            AddingHeadersToHttpClientRequest(requestHeaders, httpClient);

            if (body != null)
            {
                var jsonSerialize = body.JsonSerialize();
                request.Content = new StringContent(jsonSerialize, Encoding.UTF8, "application/json");
            }

            var response = httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
                return default;

            var result = response.Content.ReadAsStringAsync().Result.JsonDeserialize<T>();

            AppendCookieToHttpContext(cookieName, cookies, requestUri);

            return result;
        }
    }
}
