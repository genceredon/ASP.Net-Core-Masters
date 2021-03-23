using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        /// <summary>
        /// Error
        /// </summary>
        [Route("/error")]
        [HttpGet]
        public IActionResult Error() => Problem();
    }
}
