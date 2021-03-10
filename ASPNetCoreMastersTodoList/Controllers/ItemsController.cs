using Microsoft.AspNetCore.Mvc;
using Services;
using Microsoft.Extensions.Logging;
using ASPNetCoreMastersTodoList.Api.ApiModels;
using Services.DTO;
using ASPNetCoreMastersTodoList.Api.BindingModels;
using ASPNetCoreMastersTodoList.Api.Filters;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [GlobalTimeElapsedAsyncFilter]
    public class ItemsController : ControllerBase, IAsyncActionFilter
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

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);

            return Ok("Successfully Deleted!");
        }

        [NonAction]
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.ContainsKey("id"))
            {
                var id = (int)context.ActionArguments["id"];
                var result = await _service.GetAsync(id);

                if (result.Id == 0)
                {
                    context.Result = new NotFoundResult();
                    return;
                }
            }

            await next();
        }
    }
}
