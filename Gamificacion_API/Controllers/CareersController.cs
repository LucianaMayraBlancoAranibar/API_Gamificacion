using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gamificacion_API.Data;
using Gamificacion_API.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Gamificacion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CareersController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public CareersController(BdgamificacionContext context)
        {
            _context = context;
        }

        // GET: api/Careers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Career>>> GetCareers()
        {
            if (_context.Careers == null)
            {
                return NotFound();
            }
            return await _context.Careers.ToListAsync();
        }

        // GET: api/Careers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Career>> GetCareer(int id)
        {
            var opcion = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var career = await _context.Careers
                       .Include(e => e.IdDepartmentNavigation)
                       .FirstOrDefaultAsync(e => e.IdCareer == id);

            if (career == null)
            {
                return NotFound();
            }

            var json = JsonSerializer.Serialize(career, opcion);

            return career;
        }

        // PUT: api/Careers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> updateCareer(int id, Career upDatecareer)
        {
            if (id != upDatecareer.IdCareer)
            {
                return BadRequest();
            }

            var existingCareer = await _context.Careers.FindAsync(id);

            if (existingCareer == null)
            {
                return NotFound(); // El departamento con el ID especificado no existe
            }

            existingCareer.CareerName = upDatecareer.CareerName;
            existingCareer.IdDepartment = upDatecareer.IdDepartment; // Actualiza la propiedad IdFaculty

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CareerExists(id))
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

        // POST: api/Careers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Career>> PostCareer(Career career)
        {
            if (career == null)
            {
                return BadRequest();
            }

            try
            {
                _context.Careers.Add(career);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }


            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var json = JsonSerializer.Serialize(career, options);

            return Content(json, "application/json");
        }

        // DELETE: api/Careers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCareer(int id)
        {
            if (_context.Careers == null)
            {
                return NotFound();
            }
            var career = await _context.Careers.FindAsync(id);
            if (career == null)
            {
                return NotFound();
            }

            _context.Careers.Remove(career);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool CareerExists(int id)
        {
            return (_context.Careers?.Any(e => e.IdCareer == id)).GetValueOrDefault();
        }
    }
}
