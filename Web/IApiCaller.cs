using System.Net.Http;

namespace Sepidar.Web
{
    public interface IApiCaller
    {
        public T Get<T>(string url);
     
        public void Post(string uri, object body = null);

        public T Post<T>(string uri, object body, bool getCookie = false);

        T Post<T>(string uri, HttpContent content, bool getCookie = false);

        public void RemoveCookie();
    }
}
