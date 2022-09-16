using System.Net;

namespace Acme.FootballTables.Server.Services
{
    public class ServiceActionResult<T> where T : class
    {
        public HttpStatusCode ResponseCode { get; private set; }
        public T ResponseBody { get; private set; }

        public ServiceActionResult(HttpStatusCode responseCode, T responseBody)
        {
            ResponseCode = responseCode;
            ResponseBody = responseBody;
        }
    }
}
