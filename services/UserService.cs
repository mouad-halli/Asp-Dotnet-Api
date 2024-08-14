using FirstAPI.interfaces;
using FirstAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace FirstAPI.services
{
    public class UserService: IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<(string? errorMsg, IdentityResult? result)> CreateUser(User newUser, string userPassword)
        {
            if (await _userManager.FindByNameAsync(newUser.UserName!) != null)
                return ("username already in use", null);
            if (await _userManager.FindByEmailAsync(newUser.Email!) != null)
                return ("email already in use", null);

            IdentityResult identity = await _userManager.CreateAsync(newUser, userPassword);

            return (null, identity);
        }
    }
}