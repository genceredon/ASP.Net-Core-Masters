using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.Extensions.Logging;
using ASPNetCoreMastersTodoList.Api.ApiModels;
using Services.DTO;
using ASPNetCoreMastersTodoList.Api.BindingModels;
using ASPNetCoreMastersTodoList.Api.Filters;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IItemService _service;

        public ItemsController(ILogger<ItemsController> logger, IItemService service)
        {
            _logger = logger;
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [Route("{*url}", Order = 999)]
        public IActionResult CatchAll()
        {
            Response.StatusCode = 404;
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var result = await _service.GetAllAsync();

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [CheckItemExists]
        public async Task<IActionResult> GetAsync(int id)
        {
            var result = await _service.GetAsync(id);

            return Ok(result);
        }

        [HttpGet("filterBy/{filters=text}")]
        public async Task<IActionResult> GetAllByFilterAsync([FromQuery] ItemByFilterDTO filters)
        {
            var result = await _service.GetAllByFilterAsync(filters);
            
            var isNullOrEmpty = result.Cast<object>().Any();

            if (!isNullOrEmpty)
            {
                return NotFound();
            }

            return Ok(result);
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
        [CheckItemExists]
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

        //Just Added extra authorization here, only with Admin Role can access the Delete API (sample only)
        //Can also add to other API

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id:int}")]
        [CheckItemExists]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);

            return Ok("Successfully Deleted!");
        }        
    }
}
