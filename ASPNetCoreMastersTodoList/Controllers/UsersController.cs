using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNetCoreMastersTodoList.Api.Models;
using Microsoft.AspNetCore.Identity;
using ASPNetCoreMastersTodoList.Api.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using ASPNetCoreMastersTodoList.Api.BindingModels;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using ASPNetCoreMastersTodoList.Api.ApiModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly Settings _settings;
        private readonly SignInManager<ASPNetCoreMastersTodoListApiUser> _signInManager;
        private readonly UserManager<ASPNetCoreMastersTodoListApiUser> _userManager;
        private readonly IEmailSender _emailSender;

        public UsersController(ILogger<UsersController> logger, IOptions<Settings> options,
            UserManager<ASPNetCoreMastersTodoListApiUser> userManager,
            SignInManager<ASPNetCoreMastersTodoListApiUser> signInManager, IEmailSender emailSender)
        {
            _logger = logger;
            _settings = options.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok(_settings.SecurityKey);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> OnPostRegisterAsync([FromBody]RegisterInputBindingModel register, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            register.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (ModelState.IsValid)
            {
                var user = new ASPNetCoreMastersTodoListApiUser { UserName = register.Input.Email, Email = register.Input.Email };
                var result = await _userManager.CreateAsync(user, register.Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    
                    var callbackUrl = $"https://localhost:44387/users/ConfirmEmail?userId={user.Id}&code={code}";

                    var response = new ResponseModel
                    {
                        StatusMessage = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>."
                    };

                    await _emailSender.SendEmailAsync(register.Input.Email, "Confirm your email", response.StatusMessage);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return Ok(response.StatusMessage);
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Ok("Success!");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(ModelState);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> OnGetConfirmEmailAsync(string userId, string code)
        {
            var response = new ResponseModel();

            if (userId == null || code == null)
            {
                return RedirectToPage("/");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            response.StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";
            return Ok(response.StatusMessage);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> OnPostLoginAsync([FromBody] LoginInputBindingModel model, string returnUrl = null)
        {
            var response = new ResponseModel();

            returnUrl ??= Url.Content("~/");

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Input.Email, model.Input.Password, model.Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    response.StatusMessage = "User logged in.";

                    _logger.LogInformation("User logged in.");
                    return Ok(response.StatusMessage);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    response.ErrorMessage = "User account locked out.";
                    _logger.LogWarning(response.ErrorMessage);
                    return BadRequest(response.ErrorMessage);
                }
                else
                {
                    response.ErrorMessage = "Invalid login attempt.";
                    ModelState.AddModelError(string.Empty, response.ErrorMessage);
                    return BadRequest(response.ErrorMessage);
                }
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(ModelState);
        }
    }
}
