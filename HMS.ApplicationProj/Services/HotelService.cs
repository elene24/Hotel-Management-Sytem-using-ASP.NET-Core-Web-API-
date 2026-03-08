using HMS.ApplicationProj.DTOS;
using HMS.DomainProj.Entities;
using HMS.InfrastructureProj.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.ApplicationProj.Services
{
    public class HotelService
    {
        private readonly HotelRepository _repo;

        public HotelService(HotelRepository repo)
        {
            _repo = repo;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    Encoding.UTF8.GetBytes(password)));
        }

        #region HOTEL METHODS

        public async Task<List<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = await _repo.GetAllHotelsAsync();

            return hotels.Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Country = h.Country,
                Address = h.Address,
                Rating = h.Rating
            }).ToList();
        }

        public async Task<HotelDto?> GetHotelByIdAsync(int hotelId)
        {
            var hotel = await _repo.GetHotelByIdAsync(hotelId);
            if (hotel == null) return null;

            return new HotelDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                City = hotel.City,
                Country = hotel.Country,
                Address = hotel.Address,
                Rating = hotel.Rating
            };
        }

        public async Task<HotelDto> CreateHotelAsync(HotelDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Hotel name is required.");

            if (string.IsNullOrWhiteSpace(dto.City))
                throw new ArgumentException("City is required.");

            if (string.IsNullOrWhiteSpace(dto.Country))
                throw new ArgumentException("Country is required.");

            if (string.IsNullOrWhiteSpace(dto.Address))
                throw new ArgumentException("Address is required.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var hotel = new Hotel
            {
                Name = dto.Name,
                City = dto.City,
                Country = dto.Country,
                Address = dto.Address,
                Rating = dto.Rating
            };

            await _repo.AddHotelAsync(hotel);

            return new HotelDto
            {
                Id = hotel.Id,
                Name = hotel.Name,
                City = hotel.City,
                Country = hotel.Country,
                Address = hotel.Address,
                Rating = hotel.Rating
            };
        }

        public async Task<bool> UpdateHotelAsync(int hotelId, HotelDto dto)
        {
            var hotel = await _repo.GetHotelByIdAsync(hotelId);
            if (hotel == null) return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Hotel name is required.");

            if (string.IsNullOrWhiteSpace(dto.City))
                throw new ArgumentException("City is required.");

            if (string.IsNullOrWhiteSpace(dto.Country))
                throw new ArgumentException("Country is required.");

            if (string.IsNullOrWhiteSpace(dto.Address))
                throw new ArgumentException("Address is required.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            hotel.Name = dto.Name;
            hotel.City = dto.City;
            hotel.Country = dto.Country;
            hotel.Address = dto.Address;
            hotel.Rating = dto.Rating;

            await _repo.UpdateHotelAsync(hotel);
            return true;
        }

        public async Task<bool> DeleteHotelAsync(int hotelId)
        {
            var hotel = await _repo.GetHotelByIdAsync(hotelId);
            if (hotel == null) return false;

            if (hotel.Rooms.Any() || hotel.Managers.Any())
                return false;

            await _repo.DeleteHotelAsync(hotel);
            return true;
        }

        public async Task<List<HotelDto>> FilterHotelsAsync(string? country, string? city, int? rating)
        {
            var hotels = await _repo.FilterHotelsAsync(country, city, rating);

            return hotels.Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Country = h.Country,
                Address = h.Address,
                Rating = h.Rating
            }).ToList();
        }

        #endregion

        #region ROOM METHODS

        public async Task<RoomDto> AddRoomToHotelAsync(int hotelId, RoomDto dto)
        {
            var hotel = await _repo.GetHotelByIdAsync(hotelId);
            if (hotel == null)
                throw new ArgumentException("Hotel not found.");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Room name is required.");

            if (dto.Price <= 0)
                throw new ArgumentException("Room price must be greater than 0.");

            var room = new Room
            {
                Name = dto.Name,
                Price = dto.Price,
                HotelId = hotelId
            };

            await _repo.AddRoomAsync(room);

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Price = room.Price
            };
        }

        public async Task<RoomDto?> GetRoomAsync(int hotelId, int roomId)
        {
            var room = await _repo.GetRoomAsync(hotelId, roomId);
            if (room == null) return null;

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Price = room.Price
            };
        }

        public async Task<bool> UpdateRoomAsync(int hotelId, int roomId, RoomDto dto)
        {
            var room = await _repo.GetRoomAsync(hotelId, roomId);
            if (room == null) return false;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Room name is required.");

            if (dto.Price <= 0)
                throw new ArgumentException("Room price must be greater than 0.");

            room.Name = dto.Name;
            room.Price = dto.Price;

            await _repo.UpdateRoomAsync(room);
            return true;
        }

        public async Task<bool> DeleteRoomAsync(int hotelId, int roomId)
        {
            var room = await _repo.GetRoomAsync(hotelId, roomId);
            if (room == null) return false;

            if (await _repo.HasActiveOrFutureReservationsAsync(roomId))
                return false;

            await _repo.DeleteRoomAsync(room);
            return true;
        }

        public async Task<List<RoomDto>> SearchRoomsAsync(int hotelId, decimal? minPrice, decimal? maxPrice, DateTime? date)
        {
            var rooms = await _repo.SearchRoomsAsync(hotelId, minPrice, maxPrice, date);

            return rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Price = r.Price
            }).ToList();
        }

        #endregion

        #region MANAGER METHODS

        public async Task<ManagerDto> AddManagerToHotelAsync(int hotelId, ManagerDto dto)
        {
            var hotel = await _repo.GetHotelByIdAsync(hotelId);
            if (hotel == null)
                throw new ArgumentException("Hotel not found.");

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ArgumentException("FirstName is required.");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("LastName is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new ArgumentException("PhoneNumber is required.");

            if (string.IsNullOrWhiteSpace(dto.PersonalNumber))
                throw new ArgumentException("PersonalNumber is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required.");

            var manager = new Manager
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PersonalNumber = dto.PersonalNumber,
                HotelId = hotelId,
                PasswordHash = HashPassword(dto.Password),
                Role = "Manager"
            };

            await _repo.AddManagerAsync(manager);

            return new ManagerDto
            {
                Id = manager.Id,
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Email = manager.Email,
                PhoneNumber = manager.PhoneNumber,
                PersonalNumber = manager.PersonalNumber,
                Password = string.Empty
            };
        }

        public async Task<bool> UpdateManagerAsync(int hotelId, int managerId, ManagerDto dto)
        {
            var manager = await _repo.GetManagerAsync(hotelId, managerId);
            if (manager == null) return false;

            if (string.IsNullOrWhiteSpace(dto.FirstName))
                throw new ArgumentException("FirstName is required.");

            if (string.IsNullOrWhiteSpace(dto.LastName))
                throw new ArgumentException("LastName is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
                throw new ArgumentException("PhoneNumber is required.");

            manager.FirstName = dto.FirstName;
            manager.LastName = dto.LastName;
            manager.Email = dto.Email;
            manager.PhoneNumber = dto.PhoneNumber;

            await _repo.UpdateManagerAsync(manager);
            return true;
        }

        public async Task<bool> DeleteManagerAsync(int hotelId, int managerId)
        {
            var manager = await _repo.GetManagerAsync(hotelId, managerId);
            if (manager == null) return false;

            if (!await _repo.CanDeleteManagerAsync(hotelId, managerId))
                return false;

            await _repo.DeleteManagerAsync(manager);
            return true;
        }

        #endregion

        #region RESERVATION METHODS

        public async Task<ReservationDto> CreateReservationAsync(int hotelId, ReservationDto dto)
        {
            if (dto.GuestId <= 0)
                throw new ArgumentException("GuestId is required.");

            if (dto.RoomIds == null || !dto.RoomIds.Any())
                throw new ArgumentException("At least one room must be selected.");

            if (dto.CheckInDate.Date < DateTime.Today)
                throw new ArgumentException("Check-in date cannot be in the past.");

            if (dto.CheckOutDate <= dto.CheckInDate)
                throw new ArgumentException("Check-out must be after check-in.");

            if (!await _repo.AreRoomsAvailableAsync(dto.RoomIds, dto.CheckInDate, dto.CheckOutDate))
                throw new InvalidOperationException("Rooms are not available.");

            var reservation = new Reservation
            {
                GuestId = dto.GuestId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate,
                ReservationRooms = dto.RoomIds
                    .Select(r => new ReservationRoom { RoomId = r })
                    .ToList()
            };

            await _repo.AddReservationAsync(reservation);

            return new ReservationDto
            {
                Id = reservation.Id,
                GuestId = reservation.GuestId,
                CheckInDate = reservation.CheckInDate,
                CheckOutDate = reservation.CheckOutDate,
                RoomIds = reservation.ReservationRooms.Select(rr => rr.RoomId).ToList()
            };
        }

        public async Task<bool> UpdateReservationAsync(int hotelId, int reservationId, ReservationDto dto)
        {
            var reservation = await _repo.GetReservationAsync(hotelId, reservationId);
            if (reservation == null) return false;

            if (dto.CheckInDate.Date < DateTime.Today)
                throw new ArgumentException("Check-in date cannot be in the past.");

            if (dto.CheckOutDate <= dto.CheckInDate)
                throw new ArgumentException("Check-out must be after check-in.");

            var existingRoomIds = reservation.ReservationRooms
                .Select(rr => rr.RoomId)
                .ToList();

            if (!await _repo.AreRoomsAvailableAsync(existingRoomIds, dto.CheckInDate, dto.CheckOutDate, reservationId))
                return false;

            reservation.CheckInDate = dto.CheckInDate;
            reservation.CheckOutDate = dto.CheckOutDate;

            await _repo.UpdateReservationAsync(reservation);
            return true;
        }

        public async Task<bool> CancelReservationAsync(int hotelId, int reservationId)
        {
            var reservation = await _repo.GetReservationAsync(hotelId, reservationId);
            if (reservation == null) return false;

            await _repo.DeleteReservationAsync(reservation);
            return true;
        }

        public async Task<List<ReservationDto>> SearchReservationsAsync(int hotelId, int? guestId, int? roomId, DateTime? date)
        {
            var reservations = await _repo.SearchReservationsAsync(hotelId, guestId, roomId, date);

            return reservations.Select(r => new ReservationDto
            {
                Id = r.Id,
                GuestId = r.GuestId,
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate,
                RoomIds = r.ReservationRooms?.Select(rr => rr.RoomId).ToList()
            }).ToList();
        }

        #endregion
    }
}