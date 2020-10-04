using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using QoHash.Web.Helpers;
using QoHash.Web.Models;

namespace APIConsume.Controllers
{
    public class HomeController : Controller
    {
	    private readonly IHttpRequestMessageCreator _requestMessageCreator;
	    private readonly string _baseApiUrl;

        public HomeController(IHttpRequestMessageCreator requestMessageCreator, IConfiguration configuration)
        {
		    _baseApiUrl = configuration["BaseApiUrl"];
            _requestMessageCreator = requestMessageCreator;
	    }

	    public IActionResult Index()
        {
            return View();
        }

        public ViewResult GetReservation() => View();

        [HttpGet]
        public async Task<IActionResult> Scan(string path)
        {
	        var scanResult = new ScanResult();
            using (var httpClient = new HttpClient())
            {
	            using (var request = _requestMessageCreator.Create(HttpMethod.Get, $"{_baseApiUrl}/api/DiskAgent/Scan", new ScanDto{Path=path}, "1.0"))
	            using (var response = await httpClient.SendAsync(request))
	            {
		            if (response.IsSuccessStatusCode)
		            {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        scanResult = JsonConvert.DeserializeObject<ScanResult>(apiResponse);
                    }
		            else
		            {
			            ViewBag.ErrorMessage = "An error occurred on the server";
			            return View();
                    }
	            }
            }
            return View(scanResult);
        }
    }
}