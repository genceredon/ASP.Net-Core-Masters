using ASPNetCoreMastersTodoList.Api.ApiModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ASPNetCoreMastersTodoList.Api.BindingModels
{
    public class LoginInputBindingModel
    {
        [BindProperty]
        public LoginInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
