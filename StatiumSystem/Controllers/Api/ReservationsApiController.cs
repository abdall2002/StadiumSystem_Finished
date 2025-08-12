using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatiumSystem.Models;
using System.Security.Claims;

namespace StatiumSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ReservationsApiController : ControllerBase
    {
        private readonly StadiumDbContext _context;
        private readonly IMapper _mapper;
  

        public ReservationsApiController(StadiumDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Invalid reservation data", errors = ModelState });

            
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { message = "User not found in token" });

            var stadium = await _context.Stadiums.FindAsync(dto.StadiumId);
            if (stadium == null)
                return BadRequest(new { message = "Stadium not found" });

            var conflictExists = await _context.Reservations
                .AnyAsync(r =>
                    r.StadiumId == dto.StadiumId &&
                    r.ReservationDate.Date == dto.ReservationDate.Date &&
                    !(dto.EndTime <= r.StartTime || dto.StartTime >= r.EndTime)
                );

            if (conflictExists)
                return BadRequest(new { message = "This time slot is already reserved" });

            var reservation = new Reservation 
            {
                StadiumId = dto.StadiumId,
                UserName = userName, 
                ReservationDate = dto.ReservationDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = Statusenum.Pending.ToString(),
                
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            var reservationDto = _mapper.Map<ReservationDTO>(reservation);

            return Ok(new
            {
                message = "Reservation created successfully",
                reservationDto
            });
        }

        [HttpGet("/api/ReservationsApi/MyReservations")]
        public async Task<IActionResult> MyReservations()
        {
            var userName = User.Identity.Name;

            var reservations = await _context.Reservations
                .Include(r => r.Stadium)
                .Where(r => r.UserName == userName)
                .OrderByDescending(r => r.ReservationDate)
                .ThenByDescending(r => r.StartTime)
                .ToListAsync();

            var reservationDTOs = _mapper.Map<List<ReservationViewDTO>>(reservations);

            return Ok(reservationDTOs); 
        }

        [HttpGet("AllReservations")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _context.Reservations
                .Include(r => r.Stadium)
                .OrderByDescending(r => r.ReservationDate)
                .ThenByDescending(r => r.StartTime)
                .ToListAsync();

            var reservationDTOs = reservations.Select(r => new ReservationViewDTO
            {
                Id = r.Id,
                StadiumName = r.Stadium.Name,
                UserName = r.UserName, 
                ReservationDate = r.ReservationDate,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                Status = r.Status
            }).ToList();

            return Ok(reservationDTOs);
        }


        [HttpPut("UpdateStatus/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDTO model)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound();

            reservation.Status = model.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status updated successfully" });
        }

        [HttpPut("Cancel/{id}")]
        [Authorize]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Reservation not found" });

            var username = User.Identity?.Name;
            if (reservation.UserName != username && !User.IsInRole("Admin"))
                return Forbid();

            if (reservation.Status == "Pending" || reservation.Status == "Approved")
            {
                reservation.Status = "Cancelled";
                _context.Update(reservation);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation cancelled successfully" });
            }
            else
            {
                return BadRequest(new { message = "You cannot cancel this reservation" });
            }
        }

    }
}
