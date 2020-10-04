using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace QoHash.Web.Helpers
{
	public class HttpRequestMessageCreator : IHttpRequestMessageCreator
    {
        public HttpRequestMessage Create<T>(HttpMethod method, string requestUri, T content, string version)
        {
            var request = new HttpRequestMessage(method, requestUri)
            {
                Headers = { { "X-Version", version } },
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };
            return request;
        }
    }
}
