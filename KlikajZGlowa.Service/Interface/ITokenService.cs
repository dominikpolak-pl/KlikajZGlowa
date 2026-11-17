using KlikajZGlowa.Model;
using KlikajZGlowa.Service.DTO.ApplicationUser;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.Interface
{
    public interface ITokenService
    {
        Task<string> CreateAccessTokenAsync(ApplicationUser user);
        string CreateRefreshToken();
    }
}
