using Microsoft.AspNetCore.Mvc;
using Services;
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
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Authorize]
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IItemService _service;
        private readonly UserManager<ASPNetCoreMastersTodoListApiUser> _userManager;
        private readonly IAuthorizationService _authService;

        public ItemsController(ILogger logger, IItemService service,
            UserManager<ASPNetCoreMastersTodoListApiUser> userManager,
            IAuthorizationService authService)
        {
            _logger = logger;
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _userManager = userManager;
            _authService = authService;
        }

        /// <summary>
        /// The general catch-all error
        /// </summary>
        [Route("{*url}", Order = 999)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CatchAll()
        {
            _logger.Error("{methodName} -- An error occurred. URL not found.", nameof(CatchAll));
            Response.StatusCode = 404;
            return NotFound();
        }

        /// <summary>
        /// Get all TodoItem.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllTodoAsync()
        {
            _logger.Information("Calling GetAllTodoListAsync() Service.");
            var result = await _service.GetAllTodoListAsync();

            if (result == null)
            {
                _logger.Error("{methodName} -- An error occurred. Returns null result", nameof(GetAllTodoAsync));

                return NotFound();
            }

            _logger.Information("{methodName} executed successfully.", nameof(GetAllTodoAsync));
            return Ok(result);
        }

        /// <summary>
        /// Get a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("{id:int}")]
        [CheckItemExists]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTodoAsync(int id)
        {
            _logger.Information("Calling GetTodoDetailsAsync() Service.");

            var result = await _service.GetTodoDetailsAsync(id);
            
            if (result == null)
            {
                _logger.Error("{methodName} -- An error occurred. Returns null result", nameof(GetTodoAsync));

                return NotFound();
            }

            _logger.Information("{methodName} executed successfully.", nameof(GetTodoAsync));
            return Ok(result);
        }

        /// <summary>
        /// Get a specific TodoItem with filter.
        /// </summary>
        /// <param name="filters"></param>
        [HttpGet("filterBy/{filters=text}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllByFilterAsync([FromQuery] ItemByFilterDTO filters)
        {
            _logger.Information("Calling GetAllByFilterAsync() Service.");

            var result = await _service.GetAllByFilterAsync(filters);
           
            if(result == null)
            {
                _logger.Error("{methodName} -- An error occurred. Returns null result", nameof(GetAllByFilterAsync));

                return NotFound();
            }

            var isNullOrEmpty = result.Cast<object>().Any();

            if (!isNullOrEmpty)
            {
                _logger.Error("{methodName} -- An error occurred. Returns null result", nameof(GetAllByFilterAsync));

                return NotFound();
            }

            _logger.Information("{methodName} executed successfully.", nameof(GetAllByFilterAsync));
            return Ok(result);
        }

        /// <summary>
        /// Add a specific TodoItem.
        /// </summary>
        /// <param name="itemCreateModel"></param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddTodoItem([FromBody] ItemCreateApiModel itemCreateModel)
        {
            var mappedObj = new ItemDTO
            {
                Todo = itemCreateModel.Todo
            };

            DomainModels.ItemResponse response;

            var name = ((ClaimsIdentity)User.Identity).Name;
            var user = await _userManager.FindByNameAsync(name);
            
            if(user != null || mappedObj != null)
            {
                _logger.Information("Calling AddTodoItemAsync() Service.");

                response = await _service.AddTodoItemAsync(mappedObj, user);

                if(response.Status != "Success")
                {
                    _logger.Error("{methodName} -- An error occurred. {message}", nameof(AddTodoItem), response.Message);
                    return BadRequest(response);
                }
            }
            else
            {
                _logger.Error("{methodName} -- An error occurred.", nameof(AddTodoItem));

                return NotFound();
            }

            _logger.Information("{methodName} executed successfully.", nameof(AddTodoItem));
            return Ok(response);
        }

        /// <summary>
        /// Updates a specific TodoItem.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemUpdateModel"></param>
        [HttpPut("{id:int}")]
        [CheckItemExists]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTodoItem(int id,
            [FromBody] ItemUpdateBindingModel itemUpdateModel)
        {
            var mappedObj = new ItemDTO
            {
                Id = id,
                Todo = itemUpdateModel.Todo
            };

            DomainModels.ItemResponse response;

            _logger.Information("Calling GetTodoDetailsAsync() Service.");
            var todoItem = await _service.GetTodoDetailsAsync(id);
            
            if (todoItem != null)
            {
                _logger.Information("Calling AuthorizeAsync() Service.");
                var authResult = await _authService.AuthorizeAsync(User, new TodoList() { CreatedBy = todoItem.CreatedBy }, "CanEditTodoItems");

                if (!authResult.Succeeded)
                {
                    _logger.Error("{methodName} -- An error occurred. Unauthorized User.", nameof(UpdateTodoItem));
                    return new UnauthorizedResult();
                }

                _logger.Information("Calling UpdateTodoItemAsync() Service.");
                response = await _service.UpdateTodoItemAsync(mappedObj);

                if (response.Status != "Success")
                {
                    _logger.Error("{methodName} -- An error occurred. {message}", nameof(UpdateTodoItem), response.Message);
                    return BadRequest(response);
                }
            }
            else
            {
                _logger.Error("{methodName} -- An error occurred.", nameof(UpdateTodoItem));
                return NotFound();
            }

            _logger.Information("{methodName} executed successfully.", nameof(UpdateTodoItem));
            return Ok(response);
        }

        /// <summary>
        /// Deletes a specific TodoItem.
        /// </summary>
        /// <param name="id"></param> 
        [HttpDelete("{id:int}")]
        [CheckItemExists]
        public async Task<IActionResult> DeleteTodoItem(int id)
        {
            var mappedObj = new ItemDTO
            {
                Id = id,
            };

            _logger.Information("Calling DeleteTodoItemAsync() Service.");
            var response = await _service.DeleteTodoItemAsync(mappedObj);
            if (response.Status != "Success")
            {
                _logger.Error("{methodName} -- An error occurred. {message}", nameof(DeleteTodoItem), response.Message);
                return BadRequest(response);
            }

            _logger.Information("{methodName} executed successfully.", nameof(DeleteTodoItem));
            return Ok(response);
        }
    }
}
