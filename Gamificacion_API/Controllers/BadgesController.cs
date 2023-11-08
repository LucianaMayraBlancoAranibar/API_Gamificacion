using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gamificacion_API.Data;
using Gamificacion_API.Models;

namespace Gamificacion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public BadgesController(BdgamificacionContext context)
        {
            _context = context;
        }

    
        // POST: api/Badges/CreateDefaults

        [HttpPost("CreateDefaults")]
        public async Task<ActionResult<IEnumerable<DefaultBadgeCreationDto>>> PostDefaultBadges([FromBody] BadgeCreationRequestDto badgeCreationRequest)
        {
            var administrator = await _context.Admnistrators.FindAsync(badgeCreationRequest.AdministratorId);
            if (administrator == null)
            {
                return NotFound("Administrador no encontrado.");
            }

            var existingBadges = await _context.Badges
                .Where(b => b.IdTypeAchivement == badgeCreationRequest.IdTypeAchievement)
                .ToListAsync();

            if (existingBadges.Any())
            {
                return BadRequest("Ya existen badges para este tipo de logro.");
            }
            var badgeDetails = new[]
            {
                ("Inicial", 0, "/badge_inicial.png"),
                ("Principiante", 100, "/badge_principiante.png"),
                ("Intermedio", 200, "/badge_intermedio.png"),
                ("Avanzado", 300, "/badge_avanzado.png"),
                ("Experto", 400, "/badge_experto.png"),
                ("Maestro", 500, "/badge_maestro.png")
                
            };

            var badgesToCreate = badgeDetails.Select(detail => new Badge
            {
                BadgeName = badgeCreationRequest.BadgeName,
                BadgeLevel = detail.Item1,
                Points = detail.Item2,
                ImagePath = detail.Item3,
                IdTypeAchivement = badgeCreationRequest.IdTypeAchievement,
                IdAdministrator = badgeCreationRequest.AdministratorId
            }).ToList();

            _context.Badges.AddRange(badgesToCreate);
            await _context.SaveChangesAsync();
            for (int i = 0; i < badgesToCreate.Count - 1; i++)
            {
              
                badgesToCreate[i].NextLevelBadgeId = badgesToCreate[i + 1].IdBadge;
            }
            // El último badge, que es 'Maestro', no tiene un siguiente nivel.

            badgesToCreate[badgesToCreate.Count - 1].NextLevelBadgeId = null;

            // Guarda los cambios de NextLevelBadgeId en la base de datos.
            _context.Badges.UpdateRange(badgesToCreate);
            await _context.SaveChangesAsync();

            var badgesResponseDto = badgesToCreate.Select(badge => new DefaultBadgeCreationDto
            {
                BadgeName = badge.BadgeName,
                BadgeLevel = badge.BadgeLevel,
                Points = badge.Points,
                ImagePath = badge.ImagePath,
                IdTypeAchievement = badge.IdTypeAchivement
              
            }).ToList();

            return CreatedAtAction(nameof(GetBadges), new { id = badgeCreationRequest.AdministratorId }, badgesResponseDto);
        }

        // GET: api/Badges
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Badge>>> GetBadges()
        {
          if (_context.Badges == null)
          {
              return NotFound();
          }
            return await _context.Badges.ToListAsync();
        }

        // GET: api/Badges/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Badge>> GetBadge(int id)
        {
          if (_context.Badges == null)
          {
              return NotFound();
          }
            var badge = await _context.Badges.FindAsync(id);

            if (badge == null)
            {
                return NotFound();
            }

            return badge;
        }

        // PUT: api/Badges/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBadge(int id, Badge badge)
        {
            if (id != badge.IdBadge)
            {
                return BadRequest();
            }

            _context.Entry(badge).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BadgeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Badges
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Badge>> PostBadge(Badge badge)
        {
          if (_context.Badges == null)
          {
              return Problem("Entity set 'BdgamificacionContext.Badges'  is null.");
          }
            _context.Badges.Add(badge);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBadge", new { id = badge.IdBadge }, badge);
        }

        // DELETE: api/Badges/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBadge(int id)
        {
            if (_context.Badges == null)
            {
                return NotFound();
            }
            var badge = await _context.Badges.FindAsync(id);
            if (badge == null)
            {
                return NotFound();
            }

            _context.Badges.Remove(badge);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BadgeExists(int id)
        {
            return (_context.Badges?.Any(e => e.IdBadge == id)).GetValueOrDefault();
        }
    }
}
