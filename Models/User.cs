using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Models
{
    // IdentityUser  is the base class provided by ASP.NET Identity that includes default properties like username, email, password ....
    public class User: IdentityUser
    {
        // these are custome properties on top of the default ones
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

    }
}
