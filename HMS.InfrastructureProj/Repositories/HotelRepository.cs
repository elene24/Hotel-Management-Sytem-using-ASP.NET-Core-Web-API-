using System;
using System.Collections.Generic;
using System.Linq;
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

        #region HOTEL METHODS

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

        #endregion

        #region ROOM METHODS

        public async Task AddRoomAsync(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }

        public async Task<Room?> GetRoomAsync(int hotelId, int roomId)
            => await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId && r.HotelId == hotelId);

        public async Task UpdateRoomAsync(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveOrFutureReservationsAsync(int roomId)
        {
            var today = DateTime.Today;
            return await _context.ReservationRooms
                .Include(rr => rr.Reservation)
                .AnyAsync(rr => rr.RoomId == roomId && rr.Reservation.CheckOutDate >= today);
        }

        public async Task<List<Room>> SearchRoomsAsync(int hotelId, decimal? minPrice, decimal? maxPrice, DateTime? date)
        {
            var query = _context.Rooms.Where(r => r.HotelId == hotelId);

            if (minPrice.HasValue)
                query = query.Where(r => r.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(r => r.Price <= maxPrice.Value);

            if (date.HasValue)
            {
                var dateValue = date.Value.Date;
                query = query.Where(r => !_context.ReservationRooms
                    .Include(rr => rr.Reservation)
                    .Where(rr => rr.Reservation.CheckInDate <= dateValue && rr.Reservation.CheckOutDate >= dateValue)
                    .Select(rr => rr.RoomId)
                    .Contains(r.Id));
            }

            return await query.ToListAsync();
        }

        #endregion

        #region MANAGER METHODS

        public async Task AddManagerAsync(Manager manager)
        {
            await _context.Managers.AddAsync(manager);
            await _context.SaveChangesAsync();
        }

        public async Task<Manager?> GetManagerAsync(int hotelId, int managerId)
            => await _context.Managers.FirstOrDefaultAsync(m => m.Id == managerId && m.HotelId == hotelId);

        public async Task UpdateManagerAsync(Manager manager)
        {
            _context.Managers.Update(manager);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteManagerAsync(Manager manager)
        {
            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CanDeleteManagerAsync(int hotelId, int managerId)
        {
            int count = await _context.Managers.CountAsync(m => m.HotelId == hotelId && m.Id != managerId);
            return count >= 1;
        }

        #endregion

        #region RESERVATION METHODS

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<Reservation?> GetReservationAsync(int hotelId, int reservationId)
            => await _context.Reservations
                .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

        public async Task UpdateReservationAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReservationAsync(Reservation reservation)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AreRoomsAvailableAsync(List<int>? roomIds, DateTime checkIn, DateTime checkOut, int? excludeReservationId = null)
        {
            if (roomIds == null || !roomIds.Any()) return true;

            var overlapping = await _context.ReservationRooms
                .Include(rr => rr.Reservation)
                .Where(rr => roomIds.Contains(rr.RoomId) &&
                             rr.Reservation.CheckInDate < checkOut &&
                             rr.Reservation.CheckOutDate > checkIn &&
                             (excludeReservationId == null || rr.Reservation.Id != excludeReservationId.Value))
                .AnyAsync();

            return !overlapping;
        }

        public async Task<List<Reservation>> SearchReservationsAsync(int hotelId, int? guestId, int? roomId, DateTime? date)
        {
            var query = _context.Reservations
                .Include(r => r.ReservationRooms)
                .ThenInclude(rr => rr.Room)
                .Include(r => r.Guest)
                .AsQueryable();

            if (guestId.HasValue)
                query = query.Where(r => r.GuestId == guestId.Value);
            if (roomId.HasValue)
                query = query.Where(r => r.ReservationRooms.Any(rr => rr.RoomId == roomId.Value));
            if (date.HasValue)
                query = query.Where(r => r.CheckInDate <= date.Value && r.CheckOutDate >= date.Value);

            return await query.ToListAsync();
        }

        public async Task<List<Hotel>> FilterHotelsAsync(string? country, string? city, int? rating)
        {
            var query = _context.Hotels.AsQueryable();

            if (!string.IsNullOrEmpty(country))
                query = query.Where(h => h.Country == country);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(h => h.City == city);

            if (rating.HasValue)
                query = query.Where(h => h.Rating == rating.Value);

            return await query.ToListAsync();
        }

        #endregion
    }
}