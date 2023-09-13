using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class DataUserDto
    {
        public string Message { get; set; }
        public bool AuthStatus { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Prueba {get; set; }
    }
}