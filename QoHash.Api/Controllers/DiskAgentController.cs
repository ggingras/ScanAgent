using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QoHash.Api.Dto;
using QoHash.Core;
using Swashbuckle.AspNetCore.Annotations;

namespace QoHash.Api.Controllers
{
	[ApiController]
	[ApiVersion("1.0")]
	[Route("Api/[controller]")]
	public class DiskAgentController : ControllerBase
	{
		private readonly IScanAgent _scanAgent;

		public DiskAgentController(IScanAgent scanAgent)
		{
			_scanAgent = scanAgent;
		}

		/// <summary>
		/// Scan a directory and retrieve a list of all files and folders sorted by size
		/// </summary>
		/// <remarks>
		/// Sample request:
		/// 
		///     Get /api/DiskAgent/Scan
		///     {
		///        {
		///			 "path" : "a valid path"
		///			}
		///     }
		/// </remarks>
		[HttpGet]
		[Route("Scan")]
		[MapToApiVersion("1.0")]
		[SwaggerResponse((int)HttpStatusCode.OK, "Returns the scan result including the collection of all items (file or folder) found ordered by size", Type = typeof(ScanResult))]
		[SwaggerResponse((int)HttpStatusCode.InternalServerError, "An error has occurred")]
		[SwaggerResponse(554, "Invalid path")]
		public async Task<IActionResult> Scan([FromBody] ScanDto scan)
		{
			try
			{
				return Ok(await _scanAgent.ScanFolderAsync(scan.Path));
			}
			catch (ArgumentException)
			{
				return StatusCode(554, "Invalid path");
			}
			catch (Exception)
			{
				return StatusCode(500, "Internal error has occurred, please retry");
			}
		}
	}
}
