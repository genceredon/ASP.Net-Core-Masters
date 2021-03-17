using System;
using Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASPNetCoreMastersTodoList.Api.Filters
{
    public class CheckItemExistsAttribute : TypeFilterAttribute
    {
        public CheckItemExistsAttribute() : base(typeof(CheckItemExistsFilter))
        { }

        private class CheckItemExistsFilter : IAsyncActionFilter
        {
            private readonly IUserService _service;

            public CheckItemExistsFilter(IUserService service)
            {
                _service = service ?? throw new ArgumentNullException(nameof(service));
            }

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
}
