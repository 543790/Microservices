using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SecondService.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{api-version:apiVersion}/[Controller]")]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet("GetDetails")]
        public async Task<IActionResult> Get()
        {
            return Ok("Azure Practise");
        }
        
    }
}
