using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASPNetCoreMastersTodoList.Api.Filters
{
    public class GlobalTimeElapsedAsyncFilter : Attribute, IAsyncActionFilter
    {
    
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine("Executing GlobalTimeElapsedAsyncFilter.OnActionExecutionAsync");

            var timer = Stopwatch.StartNew();
            await next();
            timer.Stop();

            string result = "Elapse time: " + $"{timer.Elapsed.TotalSeconds}s";

            Console.WriteLine($"GlobalTimeElapsedAsyncFilter.OnActionExecutionAsync exits successfully. {result}");
        }
    }
}
