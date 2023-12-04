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
    public class AchievementsController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public AchievementsController(BdgamificacionContext context)
        {
            _context = context;
        }

        // GET: api/Achievements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Achievement>>> GetAchievements()
        {
          if (_context.Achievements == null)
          {
              return NotFound();
          }
            return await _context.Achievements.ToListAsync();
        }

        // GET: api/Achievements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Achievement>> GetAchievement(int id)
        {
          if (_context.Achievements == null)
          {
              return NotFound();
          }
            var achievement = await _context.Achievements.FindAsync(id);

            if (achievement == null)
            {
                return NotFound();
            }

            return achievement;
        }

        // PUT: api/Achievements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAchievement(int id, [FromBody] AchievementUpdateDto achievementUpdateDto)
        {
            // Buscar el logro en la base de datos por ID
            var existingAchievement = await _context.Achievements.FindAsync(id);

            if (existingAchievement == null)
            {
                return NotFound();
            }

          
            existingAchievement.NameAchievemt = achievementUpdateDto.NameAchievemt;
            existingAchievement.Punctuation = (byte)achievementUpdateDto.Punctuation;
            existingAchievement.ProjectName = achievementUpdateDto.ProjectName;
            existingAchievement.IdTypeAchievement = achievementUpdateDto.IdTypeAchievement;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AchievementExists(id))
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


        // POST: api/Achievements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Achievement>> PostAchievement([FromBody] AchievementUpdateDto achievementCreateDto)
        {
            if (_context.Achievements == null)
            {
                return Problem("Entity set 'BdgamificacionContext.Achievements' is null.");
            }


            var newAchievement = new Achievement
            {
                NameAchievemt = achievementCreateDto.NameAchievemt,
                Punctuation = achievementCreateDto.Punctuation,
                ProjectName = achievementCreateDto.ProjectName,
                IdTypeAchievement = achievementCreateDto.IdTypeAchievement
            };

            _context.Achievements.Add(newAchievement);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Manejo de la excepción específica
                return StatusCode(500, "A database error occurred: " + ex.Message);
            }

            // Devuelve la acción para obtener el logro y el logro creado
            return CreatedAtAction(nameof(GetAchievement), new { id = newAchievement.IdAchievement }, newAchievement);
        }


        // DELETE: api/Achievements/5

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAchievement(int id)
        {
           
            var studentAchievements = _context.StudentAchievements.Where(sa => sa.IdAchievement == id).ToList();

           
            _context.StudentAchievements.RemoveRange(studentAchievements);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                
            }

       
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement != null)
            {
                _context.Achievements.Remove(achievement);
                await _context.SaveChangesAsync();
                return NoContent();
            }

            return NotFound();
        }

        private bool AchievementExists(int id)
        {
            return (_context.Achievements?.Any(e => e.IdAchievement == id)).GetValueOrDefault();
        }
    }
}
