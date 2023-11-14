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
    public class BadgeStudentsController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public BadgeStudentsController(BdgamificacionContext context)
        {
            _context = context;
        }

        [HttpPost("AssignInitialByName")]
        public async Task<ActionResult> AssignInitialBadgeToStudentByName([FromBody] AssignBadgeDto badgeData)
        {
          
            var studentExists = await _context.Students.AnyAsync(s => s.IdStudent == badgeData.StudentId);
            if (!studentExists)
            {
                return NotFound($"No se encontró un estudiante con el ID {badgeData.StudentId}.");
            }
      
            var initialBadge = await _context.Badges
                .FirstOrDefaultAsync(b => b.BadgeLevel == "Inicial" && b.BadgeName == badgeData.BadgeName);

            if (initialBadge == null)
            {
                
                return NotFound("Badge de nivel 'Inicial' con el nombre especificado no encontrado.");
            }

            // Verifica si el estudiante ya tiene este badge
            var existingBadge = await _context.BadgeStudents
                .Where(bs => bs.IdBadge == initialBadge.IdBadge && bs.IdStudent == badgeData.StudentId)
                .FirstOrDefaultAsync();

            if (existingBadge != null)
            {
               
                return BadRequest("El estudiante ya tiene asignado el badge de nivel 'Inicial' con ese nombre.");
            }

           
            var badgeStudent = new BadgeStudent
            {
                IdStudent = badgeData.StudentId,
                IdBadge = initialBadge.IdBadge,
                AccumulatedPoints = 0 
            };

            _context.BadgeStudents.Add(badgeStudent);
            await _context.SaveChangesAsync();

            var badgeStudentDto = new BadgeStudentDto
            {
                IdBadgeStudent = badgeStudent.IdBadgeStudent,
                IdStudent = badgeStudent.IdStudent,
                IdBadge = badgeStudent.IdBadge,
                AccumulatedPoints = (int)badgeStudent.AccumulatedPoints
               
            };

            return CreatedAtAction(nameof(GetBadgeStudent), new { id = badgeStudent.IdBadgeStudent }, badgeStudentDto);
        }

        public class BadgeStudentDto
        {
            public int IdBadgeStudent { get; set; }
            public int IdStudent { get; set; }
            public int IdBadge { get; set; }
            public int AccumulatedPoints { get; set; }

        }

        [HttpPost("{studentId}/UpgradeBadge")]
        public async Task<IActionResult> UpgradeStudentBadge(int studentId)
        {
            var badgeStudent = await _context.BadgeStudents
                .Include(bs => bs.IdBadgeNavigation)
                .FirstOrDefaultAsync(bs => bs.IdStudent == studentId);

            if (badgeStudent == null)
            {
                return NotFound("El estudiante no tiene ningún badge asignado.");
            }

           
            if (badgeStudent.IdBadgeNavigation.NextLevelBadgeId == null)
            {
                return BadRequest("El estudiante ya está en el nivel más alto y no puede subir de nivel.");
            }

            var nextLevelBadge = await _context.Badges.FindAsync(badgeStudent.IdBadgeNavigation.NextLevelBadgeId);

            if (nextLevelBadge == null)
            {
                return NotFound("No hay un siguiente nivel de badge definido.");
            }

            if (badgeStudent.AccumulatedPoints >= nextLevelBadge.Points)
            {
                badgeStudent.IdBadge = nextLevelBadge.IdBadge;
                badgeStudent.AccumulatedPoints = 0; 

                _context.Entry(badgeStudent).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Badge del estudiante actualizado al siguiente nivel.");
            }

            return BadRequest("El estudiante no tiene suficientes puntos para subir de nivel.");
        }


        // GET: api/BadgeStudents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BadgeStudent>>> GetBadgeStudents()
        {
          if (_context.BadgeStudents == null)
          {
              return NotFound();
          }
            return await _context.BadgeStudents.ToListAsync();
        }

        // GET: api/BadgeStudents/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BadgeStudentDto>> GetBadgeStudent(int id)
        {
            var badgeStudent = await _context.BadgeStudents
                .Select(bs => new BadgeStudentDto
                {
                    IdBadgeStudent = bs.IdBadgeStudent,
                    IdStudent = bs.IdStudent,
                    IdBadge = bs.IdBadge,
                    AccumulatedPoints = (int)bs.AccumulatedPoints
                    
                })
                .FirstOrDefaultAsync(bs => bs.IdBadgeStudent == id);

            if (badgeStudent == null)
            {
                return NotFound();
            }

            return badgeStudent;
        }

        // PUT: api/BadgeStudents/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBadgeStudent(int id, BadgeStudent badgeStudent)
        {
            if (id != badgeStudent.IdBadgeStudent)
            {
                return BadRequest();
            }

            _context.Entry(badgeStudent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BadgeStudentExists(id))
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

        // POST: api/BadgeStudents
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BadgeStudent>> PostBadgeStudent(BadgeStudent badgeStudent)
        {
          if (_context.BadgeStudents == null)
          {
              return Problem("Entity set 'BdgamificacionContext.BadgeStudents'  is null.");
          }
            _context.BadgeStudents.Add(badgeStudent);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBadgeStudent", new { id = badgeStudent.IdBadgeStudent }, badgeStudent);
        }

        // DELETE: api/BadgeStudents/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBadgeStudent(int id)
        {
            if (_context.BadgeStudents == null)
            {
                return NotFound();
            }
            var badgeStudent = await _context.BadgeStudents.FindAsync(id);
            if (badgeStudent == null)
            {
                return NotFound();
            }

            _context.BadgeStudents.Remove(badgeStudent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BadgeStudentExists(int id)
        {
            return (_context.BadgeStudents?.Any(e => e.IdBadgeStudent == id)).GetValueOrDefault();
        }
    }
}
