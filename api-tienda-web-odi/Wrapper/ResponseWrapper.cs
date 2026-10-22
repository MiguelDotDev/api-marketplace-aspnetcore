using System.Net;

namespace api_tienda_web_odi.Wrapper
{
    public class ResponseWrapper<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode Code { get; set; }
    }
}
