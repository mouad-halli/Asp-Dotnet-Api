using FirstAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    /* TO DO:
     *      - generate email confirmation code and create email confirmation Route
     *      - generate JWT for users
     *      - add Two-Factor Authentication
     */

    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerData)
        {
            try
            {

                // ModelState is a property of the ControllerBase used to represent the state of DTO binding and validation during an HTTP request
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // TO DO:
                //      - check if username or email already exists, if so throw an ERROR

                var user = new User
                {
                    FirstName = registerData.FirstName,
                    LastName = registerData.LastName,
                    Email = registerData.Email,
                    UserName = registerData.UserName
                };

                var result = await _userManager.CreateAsync(user, registerData.Password);

                if (result.Succeeded)
                {
                    //TO DO:
                    //  - Generate Email confirmation Code and Send it
                    return Ok( new { message = "registered successfully" });
                }

                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return BadRequest(ModelState);
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

                if (user == null)
                    return Unauthorized();

                var result = await _signInManager.PasswordSignInAsync(user, loginData.Password, false, false);

                if (!result.Succeeded)
                    return Unauthorized();

                return Ok( new { message = "logged in successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}