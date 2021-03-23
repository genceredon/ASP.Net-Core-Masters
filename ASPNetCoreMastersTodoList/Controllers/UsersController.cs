using Microsoft.AspNetCore.Mvc;
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
using Serilog;

namespace ASPNetCoreMastersTodoList.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly SignInManager<ASPNetCoreMastersTodoListApiUser> _signInManager;
        private readonly UserManager<ASPNetCoreMastersTodoListApiUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ILogger logger, IOptions<Settings> options,
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

        /// <summary>
        /// Register User.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /user/register
        /// 
        /// {"input" : { "username" : "usertest3", "email" : "testmail3@gmail.com", "password" : "TestPassword123!", "confirmPassword" : "TestPassword123!" } }
        /// </remarks>
        /// <param name="register"></param>
        /// <returns>A newly created user</returns>
        /// <response code="200">Returns the newly created user</response>
        /// <response code="500">If the input is null or already existing</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> OnPostRegisterAsync([FromBody] RegisterInputBindingModel register)
        {
            register.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var userNameExists = await _userManager.FindByNameAsync(register.Input.Username);
                var userEmailExists = await _userManager.FindByEmailAsync(register.Input.Email);

                if (userEmailExists != null)
                {
                    _logger.Error("{methodNameName} -- An error occurred. Email Address already exists!", nameof(OnPostRegisterAsync));

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Email Address already exists!" });
                }
                else if (userNameExists != null)
                {
                    _logger.Error("{methodNameName} -- An error occurred. Username already exists!", nameof(OnPostRegisterAsync));

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Username already exists!" });
                }

                var user = new ASPNetCoreMastersTodoListApiUser
                {
                    UserName = register.Input.Username,
                    Email = register.Input.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                _logger.Information("{methodNameName} -- User created a new account with password.", nameof(OnPostRegisterAsync));

                var result = await _userManager.CreateAsync(user, register.Input.Password);
                if (result.Succeeded)
                {
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
                        _logger.Information("{methodNameName} -- Successfully created new account with password.", nameof(OnPostRegisterAsync));

                        return Ok(new ResponseModel { Status = "Success", Message = response.Message });
                    }
                    else
                    {
                        _logger.Information("{methodNameName} -- User created successfully!", nameof(OnPostRegisterAsync));

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            _logger.Error("{methodNameName} -- An error occurred. User registration failed! Please check user details and try again.", nameof(OnPostRegisterAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User registration failed! Please check user details and try again." });
        }

        /// <summary>
        /// Register User - Admin level.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /user/register/admin
        /// 
        /// {"input" : { "username" : "usertest3", "email" : "testmail3@gmail.com", "password" : "TestPassword123!", "confirmPassword" : "TestPassword123!" } }
        /// </remarks>
        /// <param name="register"></param>
        /// <returns>A newly created user</returns>
        /// <response code="200">Returns the newly created user</response>
        /// <response code="500">If the input is null or already existing</response>
        [HttpPost("register/admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> OnPostRegisterAdminAsync([FromBody] RegisterInputBindingModel register)
        {
            register.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var userNameExists = await _userManager.FindByNameAsync(register.Input.Username);
                var userEmailExists = await _userManager.FindByEmailAsync(register.Input.Email);

                if (userEmailExists != null)
                {
                    _logger.Error("{methodNameName} -- An error occurred. Email Address already exists!", nameof(OnPostRegisterAdminAsync));
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Email Address already exists!" });

                }
                else if (userNameExists != null)
                {
                    _logger.Error("{methodNameName} -- An error occurred. Username already exists!", nameof(OnPostRegisterAdminAsync));
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
                    _logger.Information("{methodNameName} -- User created a new account with password.", nameof(OnPostRegisterAdminAsync));

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
                        _logger.Information("{methodNameName} -- Successfully created new account with password.", nameof(OnPostRegisterAdminAsync));
                        return Ok(new ResponseModel { Status = "Success", Message = response.Message });
                    }
                    else
                    {
                        _logger.Information("{methodNameName} -- User created successfully!", nameof(OnPostRegisterAdminAsync));
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            _logger.Error("{methodNameName} -- An error occurred. User registration failed! Please check user details and try again.", nameof(OnPostRegisterAsync));

            return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User registration failed! Please check user details and try again." });
        }

        /// <summary>
        /// Email Confirmation.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <response code="200">Successfully confirmed the email of the newly created user</response>
        /// <response code="400">If the userId and generated code is null</response>
        /// <response code="500">If there's an error in confirming the email</response>
        [HttpGet("{userId}/email/confirm", Name = "ConfirmEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> OnGetConfirmEmailAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Error("{methodNameName} -- An error occurred. Unable to load user with ID '{userId}'.", nameof(OnGetConfirmEmailAsync), userId);

                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (!result.Succeeded)
            {
                _logger.Error("{methodNameName} -- An error occurred. Error confirming your email.", nameof(OnGetConfirmEmailAsync), userId);

                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Error confirming your email." });
            }

            _logger.Information("{methodNameName} -- Successfully confirmed email.", nameof(OnGetConfirmEmailAsync), userId);

            return Ok(new ResponseModel { Status = "Success", Message = "Thank you for confirming your email." });
        }

        /// <summary>
        /// Login and authenticates user. Generates token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /user/login
        /// 
        /// {"input" : { "username" : "usertest3", "password" : "TestPassword123!", "rememberMe" : true } }
        /// </remarks>
        /// <response code="200">Login the newly created user</response>
        /// <response code="500">If there's an error in user login</response>
        /// <param name="model"></param>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
                        response.Message = string.Format("{methodName} -- An error occured. User with username '{username}' is not found", nameof(OnPostLoginAsync), model.Input.Username);
                        _logger.Error(response.Message);

                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                    }
                    else if (await _userManager.CheckPasswordAsync(user, model.Input.Password))
                    {
                        if (!user.EmailConfirmed)
                        {
                            response.Message = string.Format("{methodName} -- An error Occured. Email is not confirmed. Please, go to your email account.", nameof(OnPostLoginAsync));
                            _logger.Error(response.Message);

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

                            _logger.Information("{methodNameName} -- Success. User {username} logged in. Authenticated and Token generated", nameof(OnPostLoginAsync), model.Input.Username);

                            return Ok(new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(token),
                                expiration = token.ValidTo
                            });
                        }   
                    }
                    else
                    {
                        response.Message = string.Format("{methodName} -- An error occured. User password is not valid.", nameof(OnPostLoginAsync));
                        _logger.Error(response.Message);

                        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                    }
                }
                if (result.IsLockedOut)
                {
                    response.Message = string.Format("{methodName} -- An error occured. User account locked out.", nameof(OnPostLoginAsync));

                    _logger.Error(response.Message);

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                }
                else
                {
                    response.Message = string.Format("{methodName} -- An error occured.  Invalid login attempt.", nameof(OnPostLoginAsync));

                    ModelState.AddModelError(string.Empty, response.Message);
                    _logger.Error(response.Message);

                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = response.Message });
                }
            }

            _logger.Error("{methodName} -- An error occured. Unauthorized User.", nameof(OnPostLoginAsync));
            return Unauthorized();
        }
    }
}
