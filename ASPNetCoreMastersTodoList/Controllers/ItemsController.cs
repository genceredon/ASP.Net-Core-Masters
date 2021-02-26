using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Services;
using Microsoft.Extensions.Logging;
using System;
using ASPNetCoreMastersTodoList.Api.ApiModels;
using Services.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly ItemService _service = new ItemService();

        public ItemsController(ILogger<ItemsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _service.GetAll();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            //throw new Exception(); 
            var result = _service.Get(id);

            return Ok(result);
        }

        [HttpGet("filterBy/{filters=text}")]
        public IActionResult GetByFilters([FromQuery] Dictionary<string, string> filters)
        {
            var result = _service.GetByFilters(filters);

            return Ok($"Success: calling GetByFilters method. Params -- {result}");
        }

        [HttpPost]
        public IActionResult Post([FromBody] ItemCreateApiModel itemCreateModel)
        {
            var mappedObj = new ItemDTO
            {
                Text = itemCreateModel.Text
            };

            var result = _service.Post(mappedObj);

            return Ok($"Success: calling POST method - value: {result}.");
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id,
            [FromBody] ItemUpdateBindingModel itemUpdateModel)
        {
            var mappedObj = new ItemDTO
            {
                Text = itemUpdateModel.Text
            };

            var result = _service.Put(id, mappedObj);

            return Ok($"Success: calling PUT method - value: {result}.");
        }

        [HttpDelete("{itemId:int}")]
        public IActionResult Delete(int itemId)
        {
            var result = _service.Delete(itemId);
            return Ok($"Success: calling DELETE method - value: {result}.");
        }

        public IActionResult ItemsCreate([FromBody]ItemCreateApiModel itemCreateApiModel)
        {
            var mappedObj = new ItemDTO
            {
                Text = itemCreateApiModel.Text
            };

            var result = _service.Save(mappedObj);
           
            return Ok(result);
        }
    }
}
