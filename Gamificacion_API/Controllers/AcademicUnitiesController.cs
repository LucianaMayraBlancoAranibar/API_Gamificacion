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
    public class AcademicUnitiesController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public AcademicUnitiesController(BdgamificacionContext context)
        {
            _context = context;
        }


        // GET: api/AcademicUnities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AcademicUnity>>> GetAcademicUnities()
        {
            if (_context.AcademicUnities == null)
            {
                return NotFound();
            }
            return await _context.AcademicUnities.ToListAsync();
        }

        // GET: api/AcademicUnities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AcademicUnity>> GetAcademicUnity(int id)
        {
            if (_context.AcademicUnities == null)
            {
                return NotFound();
            }
            var academicUnity = await _context.AcademicUnities.FindAsync(id);

            if (academicUnity == null)
            {
                return NotFound();
            }

            return academicUnity;
        }

        // PUT: api/AcademicUnities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAcademicUnity(int id, AcademicUnity academicUnity)
        {
            if (id != academicUnity.IdAcademicUnity)
            {
                return BadRequest("El ID proporcionado en la URL no coincide con el ID del objeto.");
            }

            try
            {
                _context.Entry(academicUnity).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcademicUnityExists(id))
                {
                    return NotFound("No se encontró la unidad académica con el ID proporcionado.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AcademicUnities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AcademicUnity>> PostAcademicUnity(AcademicUnity academicUnity)
        {
            try
            {
                _context.AcademicUnities.Add(academicUnity);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetAcademicUnity", new { id = academicUnity.IdAcademicUnity }, academicUnity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/AcademicUnities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAcademicUnity(int id)
        {
            if (_context.AcademicUnities == null)
            {
                return NotFound();
            }
            var academicUnity = await _context.AcademicUnities.FindAsync(id);
            if (academicUnity == null)
            {
                return NotFound();
            }

            _context.AcademicUnities.Remove(academicUnity);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool AcademicUnityExists(int id)
        {
            return (_context.AcademicUnities?.Any(e => e.IdAcademicUnity == id)).GetValueOrDefault();
        }
    }
}
