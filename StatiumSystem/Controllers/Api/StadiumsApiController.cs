using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StatiumSystem.Data;
using StatiumSystem.Models;

namespace StatiumSystem.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumsApiController : ControllerBase
    {
        private readonly StadiumDbContext _context;

        public StadiumsApiController(StadiumDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStadiums()
        {
            var stadiums = await _context.Stadiums.ToListAsync();
            return Ok(stadiums);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Stadium>> GetStadium(int id)
        {
            var stadium = await _context.Stadiums.FindAsync(id);
            if (stadium == null)
                return NotFound();

            return stadium;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] StadiumDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var stadium = new Stadium
            {
                Name = model.Name,
                ImageUrl = model.ImageUrl
            };

            _context.Stadiums.Add(stadium);
            await _context.SaveChangesAsync();

            return Ok(stadium);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] StadiumDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.Stadiums.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = model.Name;
            existing.ImageUrl = model.ImageUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var stadium = await _context.Stadiums.FindAsync(id);
            if (stadium == null)
                return NotFound();

            _context.Stadiums.Remove(stadium);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}