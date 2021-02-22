using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;
using Microsoft.Extensions.Logging;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ILogger<ItemController> _logger;

        public ItemController(ILogger<ItemController> logger)
        {
            _logger = logger;
        }

        // GET: <ItemController>
        [HttpGet("{id:int}")]
        public IEnumerable<string> Get(int id)
        {
            //throw new Exception(); 

            var service = new ItemService();
            var result = service.GetAll(id);

            return result;
        }
   
    }
}
