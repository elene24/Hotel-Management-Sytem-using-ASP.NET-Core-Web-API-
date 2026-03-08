using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMS.ApplicationProj.DTOS;
using HMS.DomainProj.Entities;
using HMS.InfrastructureProj.Repositories;

namespace HMS.ApplicationProj.Services
{
    public class HotelService
    {
        private readonly HotelRepository _repo;

        public HotelService(HotelRepository repo)
        {
            _repo = repo;
        }

        // HOTEL METHODS
        public async Task<List<HotelDto>> GetAllHotelsAsync()
        {
            var hotels = await _repo.GetAllHotelsAsync();
            return hotels.Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Country = h.Country,
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
                Rating = hotel.Rating
            };
        }

        public async Task<HotelDto> CreateHotelAsync(HotelDto dto)
        {
            var hotel = new Hotel
            {
                Name = dto.Name,
                City = dto.City,
                Country = dto.Country,
                Rating = dto.Rating
            };

            await _repo.AddHotelAsync(hotel);

            dto.Id = hotel.Id;
            return dto;
        }

        public async Task<bool> UpdateHotelAsync(int hotelId, HotelDto dto)
        {
            var hotel = await _repo.GetHotelByIdAsync(hotelId);
            if (hotel == null) return false;

            hotel.Name = dto.Name;
            hotel.City = dto.City;
            hotel.Country = dto.Country;
            hotel.Rating = dto.Rating;

            await _repo.UpdateHotelAsync(hotel);
            return true;
        }

        public async Task<bool> DeleteHotelAsync(int hotelId)
        {
            var hotel = await _repo.GetHotelByIdAsync(hotelId);
            if (hotel == null || hotel.Rooms.Any() || hotel.Managers.Any())
                return false;

            await _repo.DeleteHotelAsync(hotel);
            return true;
        }

        // ROOM METHODS
        public async Task<RoomDto> AddRoomToHotelAsync(int hotelId, RoomDto dto)
        {
            var room = new Room
            {
                Name = dto.Name,
                Price = dto.Price,
                HotelId = hotelId
            };

            await _repo.AddRoomAsync(room);

            dto.Id = room.Id;
            return dto;
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

        // MANAGER METHODS
        public async Task<ManagerDto> AddManagerToHotelAsync(int hotelId, ManagerDto dto)
        {
            var manager = new Manager
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PersonalNumber = dto.PersonalNumber,
                HotelId = hotelId
            };

            await _repo.AddManagerAsync(manager);
            dto.Id = manager.Id;
            return dto;
        }

        // RESERVATION METHODS
        public async Task<ReservationDto> CreateReservationAsync(int hotelId, ReservationDto dto)
        {
            var reservation = new Reservation
            {
                GuestId = dto.GuestId,
                CheckInDate = dto.CheckInDate,
                CheckOutDate = dto.CheckOutDate
            };

            // Add ReservationRooms if needed
            if (dto.RoomIds != null)
            {
                reservation.ReservationRooms = dto.RoomIds.Select(rid => new ReservationRoom
                {
                    RoomId = rid
                }).ToList();
            }

            await _repo.AddReservationAsync(reservation);
            dto.Id = reservation.Id;
            return dto;
        }
    }
}
