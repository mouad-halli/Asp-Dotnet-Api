using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FirstAPI.Models;

namespace FirstAPI.interfaces
{
    public interface ITokenService
    {
        string CreateToken(List<Claim> claims);
    }
}