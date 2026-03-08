
using Microsoft.EntityFrameworkCore;
using HMS.Domain.Entities;


namespace HMS.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)
        {
        }

        public DbSet<Hotel> Hotels => Set<Hotel>();

        public DbSet<Manager> Managers => Set<Manager>();

        public DbSet<Room> Rooms => Set<Room>();

        public DbSet<Guest> Guests => Set<Guest>();

        public DbSet<Reservation> Reservations => Set<Reservation>();

        public DbSet<ReservationRoom> ReservationRooms => Set<ReservationRoom>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReservationRoom>()
                .HasKey(rr => new { rr.ReservationId, rr.RoomId });
        }
    }
}
