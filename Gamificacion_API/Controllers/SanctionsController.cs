using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gamificacion_API.Data;
using Gamificacion_API.Models;
using System.Security.Claims;

namespace Gamificacion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanctionsController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public SanctionsController(BdgamificacionContext context)
        {
            _context = context;
        }

        // GET: api/Sanctions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sanction>>> GetSanctions()
        {
          if (_context.Sanctions == null)
          {
              return NotFound();
          }
            return await _context.Sanctions.ToListAsync();
        }

        // GET: api/Sanctions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sanction>> GetSanction(int id)
        {
          if (_context.Sanctions == null)
          {
              return NotFound();
          }
            var sanction = await _context.Sanctions.FindAsync(id);

            if (sanction == null)
            {
                return NotFound();
            }

            return sanction;
        }
        [HttpPost("CreateSanction")]
        public async Task<IActionResult> CreateSanction([FromBody] SanctionCreationRequest request)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.IdStudent == request.IdStudent);

            if (student == null)
            {
                return NotFound("El estudiante no existe.");
            }

            var responsible = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == request.ResponsibleGestorId);

            if (responsible == null)
            {
                return NotFound("No se pudo determinar al gestor o administrador responsable.");
            }

            var newSanction = new Sanction
            {
                Description = request.SanctionDescription,
                Sanction1 = (byte)request.SanctionValue,
                IdStudent = student.IdStudent
            };

            if (responsible.Rol == 1) 
            {
                newSanction.IdAdministrator = responsible.IdUsuario;
            }
            else if (responsible.Rol == 2) 
            {
                newSanction.IdGestor = responsible.IdUsuario;
            }

            _context.Sanctions.Add(newSanction);

            student.Score -= request.SanctionValue;
  
            student.Score = Math.Max(0, student.Score - request.SanctionValue);

            await _context.SaveChangesAsync();

            return Ok("Sanción creada y puntaje del estudiante actualizado con éxito.");
        }

        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _context.Students
                                         .Select(s => new { s.IdStudent, FullName = s.FirstName + " " + s.LastName })
                                         .ToListAsync();
            return Ok(students);
        }


        private string GetLoggedInUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Name);
        }

        private async Task<Usuario> GetResponsibleGestor(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
        // PUT: api/Sanctions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSanction(int id, Sanction sanction)
        {
            if (id != sanction.IdSanctions)
            {
                return BadRequest();
            }

            _context.Entry(sanction).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SanctionExists(id))
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

        // POST: api/Sanctions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Sanction>> PostSanction(Sanction sanction)
        {
          if (_context.Sanctions == null)
          {
              return Problem("Entity set 'BdgamificacionContext.Sanctions'  is null.");
          }
            _context.Sanctions.Add(sanction);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSanction", new { id = sanction.IdSanctions }, sanction);
        }

        // DELETE: api/Sanctions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSanction(int id)
        {
            if (_context.Sanctions == null)
            {
                return NotFound();
            }
            var sanction = await _context.Sanctions.FindAsync(id);
            if (sanction == null)
            {
                return NotFound();
            }

            _context.Sanctions.Remove(sanction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SanctionExists(int id)
        {
            return (_context.Sanctions?.Any(e => e.IdSanctions == id)).GetValueOrDefault();
        }
    }
}
