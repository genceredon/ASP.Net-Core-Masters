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

        public ItemsController(ILogger<ItemsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var service = new ItemService();
            var result = service.GetAll();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            //throw new Exception(); 

            var service = new ItemService();
            var result = service.Get(id);

            return Ok(result);
        }

        [HttpGet("filterBy/{filters=text}")]
        public IActionResult GetByFilters([FromQuery] Dictionary<string, string> filters)
        {
            string result = string.Empty;
            foreach(var items in filters)
            {
                result = $"Key: {items.Key}, Value: {items.Value}";
            }

            return Ok($"Success: calling GetByFilters method. Params -- {result}");
        }

        [HttpPost]
        public IActionResult Post([FromBody] ItemCreateApiModel itemCreateModel)
        {
            return Ok($"Success: calling POST method - value: {itemCreateModel.Text}.");
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id,
            [FromBody] ItemUpdateBindingModel itemUpdateModel)
        {
            return Ok($"Success: calling PUT method - value: {id}, {itemUpdateModel.Text}.");
        }

        [HttpDelete("{itemId:int}")]
        public IActionResult Delete(int itemId)
        {
            return Ok($"Success: calling DELETE method - value: {itemId}.");
        }

        public IActionResult ItemsCreate([FromBody]ItemCreateApiModel itemCreateApiModel)
        {
            var mappedObj = new ItemDTO
            {
                Text = itemCreateApiModel.Text
            };

            var service = new ItemService();
            var result = service.Save(mappedObj);
           
            return Ok(result);
        }
    }
}
