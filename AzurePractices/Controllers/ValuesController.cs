using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AzurePractices.Controllers
{
    /// <summary>
    /// The rest client
    /// </summary>

    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[Controller]")]
    public class ValuesController : ControllerBase
    {
        private readonly IHttpClient _httpClient;
      
        public ValuesController(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        // GET api/values
        [HttpGet("GetDetails")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _httpClient.GetStringAsync("http://localhost:58345/api/v1/Values/GetDetails"));
        }

       
    }
}
