using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ApplicationProj.DTOS
{
    public class LoginDto
    {
        public string EmailOrPersonalNumber { get; set; } = null!; // Can be Guest personal number or Manager/Admin email
        public string Password { get; set; } = null!;
    }
}
