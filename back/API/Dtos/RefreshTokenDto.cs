using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class RefreshTokenDto
    {
        [Required]
        public string TokenExpired { get; set; }
        public string RefreshToken { get; set; }
    }
}