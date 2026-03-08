using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ApplicationProj.DTOS
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
