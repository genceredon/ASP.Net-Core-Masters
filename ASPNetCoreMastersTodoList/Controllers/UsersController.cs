using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNetCoreMastersTodoList.Api.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly Settings _settings;

        public UsersController(ILogger<UsersController> logger, IOptions<Settings> options)
        {
            _logger = logger;
            _settings = options.Value;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok(_settings.SecurityKey);
        }
    }
}
