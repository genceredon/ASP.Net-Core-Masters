using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services;
using Microsoft.Extensions.Logging;
using ASPNetCoreMastersTodoList.Api.ApiModels;
using Services.DTO;
using ASPNetCoreMastersTodoList.Api.BindingModels;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IItemService _service;

        public ItemsController(ILogger<ItemsController> logger, IItemService service)
        {
            _logger = logger;
            _service = service;
        }

        [Route("{*url}", Order = 999)]
        public IActionResult CatchAll()
        {
            Response.StatusCode = 404;
            return NotFound();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _service.GetAll();

            return result != null ? Ok(result) : (IActionResult)NotFound();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            var result = _service.Get(id);

            return result.Text != null && result.Id > 0 ? Ok(result) : (IActionResult)NotFound();
        }

        [HttpGet("filterBy/{filters=text}")]
        public IActionResult GetAllByFilters([FromQuery] ItemByFilterDTO filters)
        {
            var result = _service.GetAllByFilter(filters);
            
            var isNullOrEmpty = result.Cast<object>().Any();

            return isNullOrEmpty ? Ok(result) : (IActionResult)NotFound();
        }

        [HttpPost]
        public IActionResult Post([FromBody] ItemCreateApiModel itemCreateModel)
        {
            var mappedObj = new ItemDTO
            {
                Text = itemCreateModel.Text
            };

            _service.Add(mappedObj);
            
            return Ok("Successfully Added!");
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id,
            [FromBody] ItemUpdateBindingModel itemUpdateModel)
        {
            var mappedObj = new ItemDTO
            {
                Id = id,
                Text = itemUpdateModel.Text
            };
            
            _service.Add(mappedObj);

            return Ok("Successfully Updated!");
        }

        [HttpDelete("{itemId:int}")]
        public IActionResult Delete(int itemId)
        {
            if (itemId != 0 )
            {
                _service.Delete(itemId);
            }
            else
            {
                return NotFound();
            }

            return Ok("Successfully Deleted!");
        }
    }
}
