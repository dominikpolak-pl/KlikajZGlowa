using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.ApplicationUser
{
    public class AuthResponseDto
    {
        public string TokenType { get; set; } = "Bearer";
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
    }
}
