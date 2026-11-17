using System;
using System.Collections.Generic;
using System.Text;

namespace KlikajZGlowa.Service.DTO.Author
{
    public class CreateAuthorDto
    {
        public string ApplicationUserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
