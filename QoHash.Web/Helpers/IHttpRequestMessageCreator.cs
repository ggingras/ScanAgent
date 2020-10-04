using System.Net.Http;

namespace QoHash.Web.Helpers
{
	public interface IHttpRequestMessageCreator
	{
		HttpRequestMessage Create<T>(HttpMethod method, string requestUri, T content, string version);
	}
}