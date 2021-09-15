using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Sepidar.Web
{
    public interface IHttpCaller
    {
        public T PostAndGetCookie<T>(string uri, object body, string cookieName, IHeaderDictionary requestHeaders);

        public T Get<T>(string url);

        public T Get<T>(string url, IHeaderDictionary requestHeaders);

        public T Post<T>(string uri, object body);

        public T Post<T>(string uri, object body, IHeaderDictionary requestHeaders);

        public T Post<T>(string url, HttpContent content, IHeaderDictionary requestHeaders);

        public bool Delete(string uri, IHeaderDictionary requestHeaders);
    }
}
