using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cs_webapi.Models;

namespace cs_webapi.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}