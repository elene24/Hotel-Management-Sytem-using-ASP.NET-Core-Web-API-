using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ApplicationProj.DTOS
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PersonalNumber { get; set; } = null!; // unique
        public string PhoneNumber { get; set; } = null!;      // unique for Guests
        public string? Email { get; set; }                     // required for Managers/Admins
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "Guest";           // "Guest", "Manager", "Admin"
    }
}
