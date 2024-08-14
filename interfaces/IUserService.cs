using FirstAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace FirstAPI.interfaces
{
    public interface IUserService
    {
        Task<(string? errorMsg, IdentityResult? result)> CreateUser(User newUser, string userPassword);
    }
}