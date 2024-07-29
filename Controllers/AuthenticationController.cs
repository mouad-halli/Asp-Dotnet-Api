using FirstAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
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

        [HttpPost]
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

                IdentityResult result = await _userManager.CreateAsync(user, registerData.Password);

                if (result.Succeeded)
                    return Ok(
                    new { message = "registered successfully" }
                    );

                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginData)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var user = await _userManager.FindByEmailAsync(loginData.Email);
                _signInManager.CheckPasswordSignInAsync()
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}