using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Domain.Entities
{
    public class Hotel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null;

        public int Rating { get; set; }

        public string Country { get; set; } = null;

        public string City { get; set; } = null;

        public string Address { get; set; } = null;

        // Navigation properties

        public ICollection<Manager> Managers { get; set; } = new List<Manager>();

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
