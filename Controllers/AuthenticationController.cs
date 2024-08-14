using System.Security.Claims;
using FirstAPI.interfaces;
using FirstAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    /* TO DO:
     *      - generate email confirmation code and create email confirmation Route
     *      - generate JWT for users -> DONE
     *      - add Two-Factor Authentication
     *      - refactor microsoft-login-callback
     */

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        private readonly ILogger<AuthenticationController> _logger;

        private readonly IUserService _userService;

        public AuthenticationController(UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager, IUserService userService, ILogger<AuthenticationController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("microsoft-login")]
        public IActionResult MicrosoftLogin()
        {
            // challenge user with openId then redirect him to /api/Authentication/microsoft-login-callback
            return Challenge(new AuthenticationProperties { RedirectUri = "/api/Authentication/microsoft-login-callback" }, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("microsoft-login-callback")]
        public async Task<IActionResult> MicrosoftLoginCallback()
        {
            AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
                return Unauthorized();

            var claims = new List<Claim>();

            var lastname = authenticateResult.Principal.FindFirstValue(ClaimTypes.Surname);
            var firstname = authenticateResult.Principal.FindFirstValue(ClaimTypes.GivenName);
            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
            var microsoftProviderKey = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(microsoftProviderKey) || string.IsNullOrEmpty(email)|| string.IsNullOrEmpty(lastname) || string.IsNullOrEmpty(firstname))
                return BadRequest("claims not found");

            var microsoftUser = await _userManager.FindByLoginAsync(OpenIdConnectDefaults.AuthenticationScheme, microsoftProviderKey);

            // user microsoft account not linked
            if (microsoftUser == null)
            {
                var existingUser = await _userManager.FindByEmailAsync(email);

                // user already exist
                if (existingUser != null)
                {
                    // link user microsoft account
                    IdentityResult result = await _userManager.AddLoginAsync(existingUser, new UserLoginInfo(OpenIdConnectDefaults.AuthenticationScheme, microsoftProviderKey, "Microsoft"));
                    
                    if (result.Succeeded != true)
                        return BadRequest(result.Errors);

                    claims.Add(new (ClaimTypes.NameIdentifier, existingUser.Id));
                    claims.Add(new (ClaimTypes.Email, existingUser.Email ?? string.Empty));
                }
                // user does not exist
                else
                {
                    // create a new user
                    var newUser = new User
                    {
                        FirstName = firstname,
                        LastName = lastname,
                        Email = email,
                        UserName = email
                    };

                    IdentityResult result = await _userManager.CreateAsync(newUser);

                    if (result.Succeeded != true)
                        return BadRequest(result.Errors);

                    // link user microsoft account
                    await _userManager.AddLoginAsync(newUser, new UserLoginInfo(OpenIdConnectDefaults.AuthenticationScheme, microsoftProviderKey, "Microsoft"));
                    claims.Add(new (ClaimTypes.NameIdentifier, newUser.Id));
                    claims.Add(new (ClaimTypes.Email, newUser.Email));
                }
            }
            // user microsoft account already linked
            else
            {
                claims.Add(new (ClaimTypes.NameIdentifier, microsoftUser.Id));
                claims.Add(new (ClaimTypes.Email, microsoftUser.Email ?? string.Empty));
            }

            // generate jwt and save it inside a cookie
            string jwtToken = _tokenService.CreateToken(claims);

            Response.Cookies.Append("access_token", jwtToken, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddHours(1) 
            });

            return Ok("logged in successfully");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerData)
        {
            try
            {
                // ModelState is a property of the ControllerBase used to represent the state of DTO binding and validation during an HTTP request
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = new User
                {
                    FirstName = registerData.FirstName,
                    LastName = registerData.LastName,
                    Email = registerData.Email,
                    UserName = registerData.UserName
                };

                var (errorMsg, result) = await _userService.CreateUser(user, registerData.Password);

                if (!string.IsNullOrEmpty(errorMsg))
                    return BadRequest(errorMsg);

                if (result != null && result.Succeeded == false)
                    return BadRequest(result.Errors);

                    
                //TO DO:
                //  - Generate Email confirmation Code and Send it

                return Ok("registered successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginData)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(loginData.Email);

                if (user == null || !await _userManager.CheckPasswordAsync(user, loginData.Password))
                    return Unauthorized("invalid email or password");

                var claims = new List<Claim>
                {
                    new (ClaimTypes.NameIdentifier, user.Id),
                    new (ClaimTypes.Email, user.Email ?? string.Empty)
                };

                string jwtToken = _tokenService.CreateToken(claims);

                Response.Cookies.Append("access_token", jwtToken, new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddHours(1) 
                });

                return Ok("logged in successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // making our cookie with the name acess_token expires so the browser remove it automatically
            Response.Cookies.Append("access_token", string.Empty, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1)
            });

            return Ok("logged out successfully");
        }

    }
}