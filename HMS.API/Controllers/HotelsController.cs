using Microsoft.AspNetCore.Mvc;
using HMS.ApplicationProj.Services;
using HMS.ApplicationProj.DTOS;
using Microsoft.AspNetCore.Authorization;

namespace HMS.API.Controllers
{
    [ApiController]
    [Route("api/hotels")]
    public class HotelsController : ControllerBase
    {
        private readonly HotelService _service;

        public HotelsController(HotelService service)
        {
            _service = service;
        }

        #region HOTEL ENDPOINTS

        [HttpGet]
        public async Task<IActionResult> GetHotels(
            [FromQuery] string? country,
            [FromQuery] string? city,
            [FromQuery] int? rating)
        {
            var hotels = await _service.FilterHotelsAsync(country, city, rating);
            return Ok(hotels);
        }

        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotelById(int hotelId)
        {
            var hotel = await _service.GetHotelByIdAsync(hotelId);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateHotel([FromBody] HotelDto dto)
        {
            var hotel = await _service.CreateHotelAsync(dto);
            return CreatedAtAction(nameof(GetHotelById), new { hotelId = hotel.Id }, hotel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{hotelId}")]
        public async Task<IActionResult> UpdateHotel(int hotelId, [FromBody] HotelDto dto)
        {
            var updated = await _service.UpdateHotelAsync(hotelId, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{hotelId}")]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            var deleted = await _service.DeleteHotelAsync(hotelId);
            if (!deleted) return BadRequest("Cannot delete hotel with rooms or managers.");
            return NoContent();
        }

        #endregion

        #region ROOM ENDPOINTS

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{hotelId}/rooms")]
        public async Task<IActionResult> AddRoom(int hotelId, [FromBody] RoomDto dto)
        {
            var room = await _service.AddRoomToHotelAsync(hotelId, dto);
            return CreatedAtAction(nameof(GetRoomById), new { hotelId, roomId = room.Id }, room);
        }

        [HttpGet("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> GetRoomById(int hotelId, int roomId)
        {
            var room = await _service.GetRoomAsync(hotelId, roomId);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> UpdateRoom(int hotelId, int roomId, [FromBody] RoomDto dto)
        {
            var updated = await _service.UpdateRoomAsync(hotelId, roomId, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{hotelId}/rooms/{roomId}")]
        public async Task<IActionResult> DeleteRoom(int hotelId, int roomId)
        {
            var deleted = await _service.DeleteRoomAsync(hotelId, roomId);
            if (!deleted) return BadRequest("Cannot delete room with active or future reservations.");
            return NoContent();
        }

        [HttpGet("{hotelId}/rooms/available")]
        public async Task<IActionResult> SearchRooms(
            int hotelId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] DateTime? date)
        {
            var rooms = await _service.SearchRoomsAsync(hotelId, minPrice, maxPrice, date);
            return Ok(rooms);
        }

        #endregion

        #region MANAGER ENDPOINTS

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{hotelId}/managers")]
        public async Task<IActionResult> AddManager(int hotelId, [FromBody] ManagerDto dto)
        {
            var manager = await _service.AddManagerToHotelAsync(hotelId, dto);
            return CreatedAtAction(nameof(GetHotelById), new { hotelId }, manager);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut("{hotelId}/managers/{managerId}")]
        public async Task<IActionResult> UpdateManager(int hotelId, int managerId, [FromBody] ManagerDto dto)
        {
            var updated = await _service.UpdateManagerAsync(hotelId, managerId, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpDelete("{hotelId}/managers/{managerId}")]
        public async Task<IActionResult> DeleteManager(int hotelId, int managerId)
        {
            var deleted = await _service.DeleteManagerAsync(hotelId, managerId);
            if (!deleted) return BadRequest("Cannot delete the last manager of the hotel.");
            return NoContent();
        }

        #endregion

        #region RESERVATION ENDPOINTS

        [Authorize(Roles = "Guest")]
        [HttpPost("{hotelId}/reservations")]
        public async Task<IActionResult> CreateReservation(int hotelId, [FromBody] ReservationDto dto)
        {
            var reservation = await _service.CreateReservationAsync(hotelId, dto);
            return CreatedAtAction(nameof(GetReservationById), new { hotelId, reservationId = reservation.Id }, reservation);
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("{hotelId}/reservations/{reservationId}")]
        public async Task<IActionResult> GetReservationById(int hotelId, int reservationId)
        {
            var reservations = await _service.SearchReservationsAsync(hotelId, null, null, null);
            var reservation = reservations.FirstOrDefault(r => r.Id == reservationId);

            if (reservation == null) return NotFound();

            return Ok(reservation);
        }

        [Authorize(Roles = "Guest")]
        [HttpPut("{hotelId}/reservations/{reservationId}")]
        public async Task<IActionResult> UpdateReservation(int hotelId, int reservationId, [FromBody] ReservationDto dto)
        {
            var updated = await _service.UpdateReservationAsync(hotelId, reservationId, dto);
            if (!updated) return BadRequest("Rooms not available or reservation does not exist.");
            return NoContent();
        }

        [Authorize(Roles = "Guest")]
        [HttpDelete("{hotelId}/reservations/{reservationId}")]
        public async Task<IActionResult> CancelReservation(int hotelId, int reservationId)
        {
            var cancelled = await _service.CancelReservationAsync(hotelId, reservationId);
            if (!cancelled) return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Guest")]
        [HttpGet("{hotelId}/reservations/search")]
        public async Task<IActionResult> SearchReservations(
            int hotelId,
            [FromQuery] int? guestId,
            [FromQuery] int? roomId,
            [FromQuery] DateTime? date)
        {
            var reservations = await _service.SearchReservationsAsync(hotelId, guestId, roomId, date);
            return Ok(reservations);
        }

        #endregion
    }
}