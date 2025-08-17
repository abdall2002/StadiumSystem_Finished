using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StadiumSystemApi.Models;
using AutoMapper;
using StatiumSystem.Models;
using StatiumSystemApi.Models;

[Authorize]
public class ReservationController : Controller
{
    private readonly StadiumDbContext _context;
    private readonly IMapper _mapper;

    public ReservationController(StadiumDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: Reservations/Create
    public async Task<IActionResult> Create(int? stadiumId)
    {
        if (User.IsInRole("Admin"))
        {
            TempData["Error"] = "Admins are not allowed to make reservations.";
            return RedirectToAction("Index", "Stadium");
        }

        if (stadiumId == null) return NotFound();

        var stadium = await _context.Stadiums.FindAsync(stadiumId);
        if (stadium == null) return NotFound();

        ViewBag.StadiumName = stadium.Name;
        ViewBag.StadiumId = stadium.Id;

        return View(new ReservationDTO
        {
            StadiumId = stadium.Id,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReservationDTO reservationDTO)
    {
        if (User.IsInRole("Admin"))
        {
            TempData["Error"] = "Admins are not allowed to make reservations.";
            return RedirectToAction("Index", "Stadium");
        }


        if (!ModelState.IsValid)
        {
            var stadium = await _context.Stadiums.FindAsync(reservationDTO.StadiumId);
            ViewBag.StadiumName = stadium?.Name ?? "Unknown";
            ViewBag.StadiumId = stadium?.Id ?? 0;
            return View(reservationDTO);
        }

        var hasConflict = await _context.Reservations.AnyAsync(r =>
            r.StadiumId == reservationDTO.StadiumId &&
            r.ReservationDate == reservationDTO.ReservationDate &&
            (
                (reservationDTO.StartTime >= r.StartTime && reservationDTO.StartTime < r.EndTime) ||
                (reservationDTO.EndTime > r.StartTime && reservationDTO.EndTime <= r.EndTime) ||
                (reservationDTO.StartTime <= r.StartTime && reservationDTO.EndTime >= r.EndTime)
            )
        );

        if (hasConflict)
        {
            ModelState.AddModelError("", "This time slot conflicts with an existing reservation.");
            var stadium = await _context.Stadiums.FindAsync(reservationDTO.StadiumId);
            ViewBag.StadiumName = stadium?.Name ?? "Unknown";
            ViewBag.StadiumId = stadium?.Id ?? 0;
            return View(reservationDTO);
        }

        var reservation = _mapper.Map<Reservation>(reservationDTO);
        reservation.Status = "Pending";

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Reservation created successfully!";
        return RedirectToAction("Index", "Stadium");
    }

    [Authorize]
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

        return View(reservationDTOs);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Manage()
    {
        var reservations = await _context.Reservations.Include(r => r.Stadium)
            .OrderByDescending(r => r.ReservationDate)
            .ThenByDescending(r => r.StartTime)
            .ToListAsync();

        var reservationDTOs = _mapper.Map<List<ReservationViewDTO>>(reservations);
        return View(reservationDTOs);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();

        reservation.Status = "Approved";
        _context.Update(reservation);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Reservation approved successfully.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Reject(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null) return NotFound();

        reservation.Status = "Rejected";
        _context.Update(reservation);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Reservation rejected successfully.";
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
            return NotFound();

        if (reservation.UserName != User.Identity.Name && !User.IsInRole("Admin"))
            return Forbid();

        if (reservation.Status == "Pending" || reservation.Status == "Approved")
        {
            reservation.Status = "Cancelled";
            _context.Update(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reservation cancelled successfully.";
        }
        else
        {
            TempData["Error"] = "You cannot cancel this reservation.";
        }

        return RedirectToAction(nameof(MyReservations));
    }

}
