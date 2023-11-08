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
    public class StudentAchievementsController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public StudentAchievementsController(BdgamificacionContext context)
        {
            _context = context;
        }

        // GET: api/StudentAchievements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentAchievement>>> GetStudentAchievements()
        {
          if (_context.StudentAchievements == null)
          {
              return NotFound();
          }
            return await _context.StudentAchievements.ToListAsync();
        }

        // GET: api/StudentAchievements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentAchievement>> GetStudentAchievement(int id)
        {
          if (_context.StudentAchievements == null)
          {
              return NotFound();
          }
            var studentAchievement = await _context.StudentAchievements.FindAsync(id);

            if (studentAchievement == null)
            {
                return NotFound();
            }

            return studentAchievement;
        }
        [HttpPost("AssignAchievement")]
        public async Task<IActionResult> AssignAchievementToStudent([FromBody] AchievementAssignmentRequest request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.FirstName == request.StudentName && s.LastName == request.StudentLastName);

            var achievement = await _context.Achievements.FirstOrDefaultAsync(a => a.NameAchievemt == request.AchievementName);

            if (student == null || achievement == null)
            {
                return NotFound("El estudiante o el logro no existen.");
            }
            // Verifica si el estudiante ya tiene asignado el logro
            bool alreadyAssigned = await _context.StudentAchievements
                                                 .AnyAsync(sa => sa.IdStudent == student.IdStudent && sa.IdAchievement == achievement.IdAchievement);
            if (alreadyAssigned)
            {
                return BadRequest("El estudiante ya tiene asignado este logro.");
            }

            var studentAchievement = new StudentAchievement
            {
                IdStudent = student.IdStudent,
                IdAchievement = achievement.IdAchievement,
                StudentPoints = achievement.Punctuation
            };

            _context.StudentAchievements.Add(studentAchievement);
            // Actualizar puntos de badge
            var badgeUpdateResult = await UpdateBadgePoints(student, achievement);
            if (badgeUpdateResult is not OkResult)
            {
                return badgeUpdateResult;
            }

            student.Score += achievement.Punctuation;

            await _context.SaveChangesAsync();

            // Actualiza el rango del estudiante basado en su puntuación
            await AssignRankToStudent(student.IdStudent, student.Score);

            return Ok("Logro asignado y puntuación actualizada con éxito.");
        }

        private List<RankRule> rankRules = new List<RankRule>
        {
            new RankRule { MinScore = 0, RankId = 5 },
            new RankRule { MinScore = 100, RankId = 6 },
            new RankRule { MinScore = 200, RankId = 7 },
             new RankRule { MinScore = 300, RankId = 8 },
        };

        [HttpPut("{id}/AssignRankToStudent")]
        public async Task<IActionResult> AssignRankToStudent(int id, int newScore)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            //  para asignar automáticamente el rango
            var studentScore = newScore;

            // Encuentra el rango adecuado según las reglas
            var studentRankId = rankRules
                .Where(rule => studentScore >= rule.MinScore)
                .OrderByDescending(rule => rule.MinScore)
                .Select(rule => rule.RankId)
                .FirstOrDefault();

            // Verifica si el rango ha cambiado antes de actualizar
            if (studentRankId != student.IdRank)
            {
               
                student.IdRank = studentRankId;

                await _context.SaveChangesAsync();
            }

            return Ok("Puntaje y rango actualizados con éxito.");
        }
        private async Task<IActionResult> UpdateBadgePoints(Student student, Achievement achievement)
        {
           
            var badgeStudent = await _context.BadgeStudents
                .Include(bs => bs.IdBadgeNavigation) 
                .FirstOrDefaultAsync(bs => bs.IdStudent == student.IdStudent &&
                                           bs.IdBadgeNavigation.IdTypeAchivement == achievement.IdTypeAchievement);

            if (badgeStudent == null)
            {
                return NotFound("Badge correspondiente no encontrado para el estudiante.");
            }

            badgeStudent.AccumulatedPoints += achievement.Punctuation;
            _context.Entry(badgeStudent).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok();
        }

        // PUT: api/StudentAchievements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudentAchievement(int id, StudentAchievement studentAchievement)
        {
            if (id != studentAchievement.IdStudentAchievement)
            {
                return BadRequest();
            }

            _context.Entry(studentAchievement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentAchievementExists(id))
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

        // POST: api/StudentAchievements
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<StudentAchievement>> PostStudentAchievement(StudentAchievement studentAchievement)
        {
          if (_context.StudentAchievements == null)
          {
              return Problem("Entity set 'BdgamificacionContext.StudentAchievements'  is null.");
          }
            _context.StudentAchievements.Add(studentAchievement);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudentAchievement", new { id = studentAchievement.IdStudentAchievement }, studentAchievement);
        }

        // DELETE: api/StudentAchievements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudentAchievement(int id)
        {
            if (_context.StudentAchievements == null)
            {
                return NotFound();
            }
            var studentAchievement = await _context.StudentAchievements.FindAsync(id);
            if (studentAchievement == null)
            {
                return NotFound();
            }

            _context.StudentAchievements.Remove(studentAchievement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentAchievementExists(int id)
        {
            return (_context.StudentAchievements?.Any(e => e.IdStudentAchievement == id)).GetValueOrDefault();
        }
    }
}
