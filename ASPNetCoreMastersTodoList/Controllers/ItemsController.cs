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
using Repositories.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IItemService _service;
        private readonly UserManager<ASPNetCoreMastersTodoListApiUser> _userManager;
        private readonly IAuthorizationService _authService;

        public ItemsController(ILogger<ItemsController> logger, IItemService service,
            UserManager<ASPNetCoreMastersTodoListApiUser> userManager,
            IAuthorizationService authService)
        {
            _logger = logger;
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _userManager = userManager;
            _authService = authService;
        }

        [Route("{*url}", Order = 999)]
        public IActionResult CatchAll()
        {
            Response.StatusCode = 404;
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTodoAsync()
        {
            var result = await _service.GetAllTodoListAsync();

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [CheckItemExists]
        public async Task<IActionResult> GetTodoAsync(int id)
        {
            var result = await _service.GetTodoDetailsAsync(id);

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
        public async Task<IActionResult> AddTodoItem([FromBody] ItemCreateApiModel itemCreateModel)
        {
            var mappedObj = new ItemDTO
            {
                Todo = itemCreateModel.Todo
            };

            var name = ((ClaimsIdentity)User.Identity).Name;
            var user = await _userManager.FindByNameAsync(name);

            var response = await _service.AddTodoItemAsync(mappedObj, user);
            
            return Ok(response);
        }


        [HttpPut("{id:int}")]
        [CheckItemExists]
        public async Task<IActionResult> UpdateTodoItem(int id,
            [FromBody] ItemUpdateBindingModel itemUpdateModel)
        {
            var mappedObj = new ItemDTO
            {
                Id = id,
                Todo = itemUpdateModel.Todo
            };
            
            var todoItem = await _service.GetTodoDetailsAsync(id);
            var authResult = await _authService.AuthorizeAsync(User, new TodoList() { CreatedBy = todoItem.CreatedBy }, "CanEditTodoItems");

            if (!authResult.Succeeded)
            {
                return new UnauthorizedResult();
            }

            var response = await _service.UpdateTodoItemAsync(mappedObj);

            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        [CheckItemExists]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var mappedObj = new ItemDTO
            {
                Id = id,
            };

            var response = await _service.DeleteTodoItemAsync(mappedObj);

            return Ok(response);
        }
    }
}
