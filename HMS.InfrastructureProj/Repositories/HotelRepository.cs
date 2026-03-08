using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HMS.DomainProj.Entities;
using HMS.InfrastructureProj.Persistence;

namespace HMS.InfrastructureProj.Repositories
{
    public class HotelRepository
    {
        private readonly AppDbContext _context;

        public HotelRepository(AppDbContext context)
        {
            _context = context;
        }

        // HOTEL METHODS
        public async Task<List<Hotel>> GetAllHotelsAsync()
            => await _context.Hotels.ToListAsync();

        public async Task<Hotel?> GetHotelByIdAsync(int id)
            => await _context.Hotels
                .Include(h => h.Rooms)
                .Include(h => h.Managers)
                .FirstOrDefaultAsync(h => h.Id == id);

        public async Task AddHotelAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHotelAsync(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }

        // ROOM METHODS
        public async Task AddRoomAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task<Room?> GetRoomAsync(int hotelId, int roomId)
            => await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);

        // MANAGER METHODS
        public async Task AddManagerAsync(Manager manager)
        {
            await _context.Managers.AddAsync(manager);
            await _context.SaveChangesAsync();
        }

        // RESERVATION METHODS
        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

    }
}
