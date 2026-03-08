using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.DomainProj.Entities
{

    public class ReservationRoom
    {
        public int ReservationId { get; set; }

        public Reservation Reservation { get; set; } = null!;

        public int RoomId { get; set; }

        public Room Room { get; set; } = null!;
    }
}
