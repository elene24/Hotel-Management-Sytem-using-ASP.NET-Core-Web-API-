using Microsoft.AspNetCore.Mvc;

using HMS.ApplicationProj.Services;
using HMS.ApplicationProj.DTOS;

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

        [HttpGet]
        public async Task<IActionResult> GetAllHotels() => Ok(await _service.GetAllHotelsAsync());

        [HttpGet("{hotelId}")]
        public async Task<IActionResult> GetHotelById(int hotelId)
        {
            var hotel = await _service.GetHotelByIdAsync(hotelId);
            if (hotel == null) return NotFound();
            return Ok(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotel(HotelDto dto)
        {
            var hotel = await _service.CreateHotelAsync(dto);
            return CreatedAtAction(nameof(GetHotelById), new { hotelId = hotel.Id }, hotel);
        }

        [HttpPut("{hotelId}")]
        public async Task<IActionResult> UpdateHotel(int hotelId, HotelDto dto)
        {
            var updated = await _service.UpdateHotelAsync(hotelId, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{hotelId}")]
        public async Task<IActionResult> DeleteHotel(int hotelId)
        {
            var deleted = await _service.DeleteHotelAsync(hotelId);
            if (!deleted) return BadRequest("Cannot delete hotel with rooms or managers");
            return NoContent();
        }

        [HttpPost("{hotelId}/rooms")]
        public async Task<IActionResult> AddRoom(int hotelId, RoomDto dto)
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

        [HttpPost("{hotelId}/managers")]
        public async Task<IActionResult> AddManager(int hotelId, ManagerDto dto)
        {
            var manager = await _service.AddManagerToHotelAsync(hotelId, dto);
            return CreatedAtAction(nameof(GetHotelById), new { hotelId }, manager);
        }

        [HttpPost("{hotelId}/reservations")]
        public async Task<IActionResult> CreateReservation(int hotelId, ReservationDto dto)
        {
            var reservation = await _service.CreateReservationAsync(hotelId, dto);
            return CreatedAtAction(nameof(GetHotelById), new { hotelId }, reservation);
        }
    }
}
