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
    public class DepartmentsController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public DepartmentsController(BdgamificacionContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve
            };

            var departament = await _context.Departments
                                .Include(e => e.IdFacultyNavigation)
                                .FirstOrDefaultAsync(e => e.IdDepartment == id);

            if (departament == null)
            {
                return NotFound();
            }

            var json = JsonSerializer.Serialize(departament, options);

            return Content(json, "application/json");
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, Department updatedDepartment)
        {
            if (id != updatedDepartment.IdDepartment)
            {
                return BadRequest(); // El ID proporcionado en la URL no coincide con el ID en el cuerpo de la solicitud
            }

            var existingDepartment = await _context.Departments.FindAsync(id);

            if (existingDepartment == null)
            {
                return NotFound(); // El departamento con el ID especificado no existe
            }

            existingDepartment.DepartmentName = updatedDepartment.DepartmentName;
            existingDepartment.IdFaculty = updatedDepartment.IdFaculty; // Actualiza la propiedad IdFaculty

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Devuelve una respuesta 204 (NoContent) para indicar éxito sin contenido
        }



        //POST: api/Departments
        //To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Department>> CreateDepartment(Department department)
        {
            if (department == null)
            {
                return BadRequest();
            }

            try
            {
                _context.Departments.Add(department);
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

            var json = JsonSerializer.Serialize(department, options);

            return Content(json, "application/json");
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            if (_context.Departments == null)
            {
                return NotFound();
            }
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        private bool DepartmentExists(int id)
        {
            return (_context.Departments?.Any(e => e.IdDepartment == id)).GetValueOrDefault();
        }
    }
}
