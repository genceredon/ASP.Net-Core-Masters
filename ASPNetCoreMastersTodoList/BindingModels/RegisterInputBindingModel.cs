using ASPNetCoreMastersTodoList.Api.ApiModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace ASPNetCoreMastersTodoList.Api.BindingModels
{
    public class RegisterInputBindingModel
    {
        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
