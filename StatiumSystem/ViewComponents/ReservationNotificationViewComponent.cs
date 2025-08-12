using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatiumSystem.Data;
using StatiumSystem.Models;

public class ReservationNotificationViewComponent : ViewComponent
{
    private readonly StadiumDbContext _context;

    public ReservationNotificationViewComponent(StadiumDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var pendingCount = await _context.Reservations
            .Where(r => r.Status == "Pending")
            .CountAsync();

        return View(pendingCount);
    }
}
