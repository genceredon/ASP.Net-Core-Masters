using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Repositories.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using ASPNetCoreMastersTodoList.Api.BindingModels;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using ASPNetCoreMastersTodoList.Api.ApiModels;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ILogger<UsersController> logger, IOptions<Settings> options,
            UserManager<ASPNetCoreMastersTodoListApiUser> userManager,
            SignInManager<ASPNetCoreMastersTodoListApiUser> signInManager, IEmailSender emailSender,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _settings = options.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return Ok(_settings.SecurityKey);
        }

        [HttpPost("register")]
        public async Task<IActionResult> OnPostRegisterAsync([FromBody] RegisterInputBindingModel register)
        {
            register.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var userNameExists = await _userManager.FindByNameAsync(register.Input.Username);
                var userEmailExists = await _userManager.FindByEmailAsync(register.Input.Email);

                if (userEmailExists != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Email Address already exists!" });

                }
                else if (userNameExists != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Username already exists!" });
                }

                var user = new ASPNetCoreMastersTodoListApiUser
                {
                    UserName = register.Input.Username,
                    Email = register.Input.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var result = await _userManager.CreateAsync(user, register.Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    //Send email confirmation with code
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    string callbackUrl = Url.Link("ConfirmEmail", new { userId = user.Id, code = code });

                    var response = new ResponseModel
                    {
                        Message = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>."
                    };

                    await _emailSender.SendEmailAsync(register.Input.Email, "Confirm your email", response.Message);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return Ok(new ResponseModel { Status = "Success", Message = response.Message });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User registration failed! Please check user details and try again." });
        }

        [HttpPost("register/admin")]
        public async Task<IActionResult> OnPostRegisterAdminAsync([FromBody] RegisterInputBindingModel register)
        {
            register.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var userNameExists = await _userManager.FindByNameAsync(register.Input.Username);
                var userEmailExists = await _userManager.FindByEmailAsync(register.Input.Email);

                if (userEmailExists != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Email Address already exists!" });

                }
                else if (userNameExists != null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Username already exists!" });
                }

                var user = new ASPNetCoreMastersTodoListApiUser
                {
                    UserName = register.Input.Username,
                    Email = register.Input.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                var result = await _userManager.CreateAsync(user, register.Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                    }
                       
                    if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                    }
                       
                    if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    {
                        await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                    }

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    string callbackUrl = Url.Link("ConfirmEmail", new { userId = user.Id, code = code });

                    var response = new ResponseModel
                    {
                        Message = $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>."
                    };

                    await _emailSender.SendEmailAsync(register.Input.Email, "Confirm your email", response.Message);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return Ok(new ResponseModel { Status = "Success", Message = response.Message });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User registration failed! Please check user details and try again." });
        }

        [HttpGet("{userId}/email/confirm", Name = "ConfirmEmail")]
        public async Task<IActionResult> OnGetConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Error confirming your email." });
            }

            return Ok(new ResponseModel { Status = "Success", Message = "Thank you for confirming your email." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> OnPostLoginAsync([FromBody] LoginInputBindingModel model)
        {
            var response = new ResponseModel();

            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Input.Username, model.Input.Password, model.Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(model.Input.Username);

                    if(user == null)
                    {
                        response.Message = $"User with username '{model.Input.Username}' is not found";
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                    }
                    else if (await _userManager.CheckPasswordAsync(user, model.Input.Password))
                    {
                        if (!user.EmailConfirmed)
                        {
                            response.Message = "Email is not confirmed. Please, go to your email account.";
                            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                        }
                        else
                        {
                            var userRoles = await _userManager.GetRolesAsync(user);

                            var authClaims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                            };

                            foreach (var userRole in userRoles)
                            {
                                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                            }

                            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JWT:SecurityKey"]));

                            var token = new JwtSecurityToken(
                                issuer: _configuration["Authentication:JWT:Issuer"],
                                audience: _configuration["Authentication:JWT:Audience"],
                                expires: DateTime.Now.AddHours(3),
                                claims: authClaims,
                                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                            );

                            response.Status = "Success";
                            response.Message = "User logged in.";

                            _logger.LogInformation("User logged in.");

                            return Ok(new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expiration = token.ValidTo
                            });
                        }   
                    }
                    else
                    {
                        response.Message = "User password is not valid.";
                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                    }
                }
                if (result.IsLockedOut)
                {
                    response.Message = "User account locked out.";

                    _logger.LogWarning(response.Message);

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                }
                else
                {
                    response.Message = "Invalid login attempt.";

                    ModelState.AddModelError(string.Empty, response.Message);
                    _logger.LogWarning(response.Message);

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                }
            }

            return Unauthorized();
        }
    }
}
