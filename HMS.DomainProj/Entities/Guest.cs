using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.DomainProj.Entities
{
    public class Guest
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PersonalNumber { get; set; } = null!; // unique
        public string PhoneNumber { get; set; } = null!;    // unique

        // JWT fields
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Guest"; // default role
    }
}
