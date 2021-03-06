using System;
using Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace ASPNetCoreMastersTodoList.Api.Filters
{
    public class CheckItemExistsAttribute : TypeFilterAttribute
    {
        public CheckItemExistsAttribute() : base(typeof(CheckItemExistsFilter))
        { }

        private class CheckItemExistsFilter : IAsyncActionFilter
        {
            private readonly IItemService _service;
            private readonly ILogger _logger;

            public CheckItemExistsFilter(IItemService service, ILogger logger)
            {
                _service = service ?? throw new ArgumentNullException(nameof(service));
                _logger = logger;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                if (context.ActionArguments.ContainsKey("id"))
                {
                    var id = (int)context.ActionArguments["id"];
                    var result = await _service.GetTodoDetailsAsync(id);

                    if (result.Id == 0)
                    {
                        _logger.Error("{methodName} -- An error occurred. Item id:{id} not found.", nameof(OnActionExecutionAsync), id);

                        context.Result = new NotFoundResult();
                        return;
                    }
                }

                await next();
            }
        }
    }
}
