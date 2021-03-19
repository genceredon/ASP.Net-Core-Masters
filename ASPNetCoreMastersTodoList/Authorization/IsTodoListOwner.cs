using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNetCoreMastersTodoList.Api.Authorization
{
    public class IsTodoListOwnerRequirement : IAuthorizationRequirement { }

    public class IsTodoListOwnerHandler :
        AuthorizationHandler<IsTodoListOwnerRequirement, TodoList>
    {
        private readonly UserManager<ASPNetCoreMastersTodoListApiUser> _userManager;

        public IsTodoListOwnerHandler(UserManager<ASPNetCoreMastersTodoListApiUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            IsTodoListOwnerRequirement requirement,
            TodoList resource)
        {
            var appUser = await _userManager.GetUserAsync(context.User);
            if (appUser == null)
            {
                return;
            }

            if (resource.CreatedBy == appUser.UserName)
            {
                context.Succeed(requirement);
            }
        }
    }
}
