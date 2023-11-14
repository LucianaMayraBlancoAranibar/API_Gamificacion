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
    public class StudentsController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public StudentsController(BdgamificacionContext context)
        {
            _context = context;
        }


        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            try
            {
                var estudiantesUsuariosInfo = await _context.Students
                    .Include(g => g.IdStudentNavigation)
                    .Select(g => new StudentUsuario
                    {
                        IdStudent = g.IdStudent,
                        FirstName = g.FirstName,
                        LastName = g.LastName,
                        Score = g.Score,
                        Email = g.IdStudentNavigation.Email,
                        Password = g.IdStudentNavigation.Password,
                        Rol = g.IdStudentNavigation.Rol,
                        IdRank = g.IdRank,
                        IdAcademicUnity = g.IdStudentNavigation.IdAcademicUnity,
                        IdCareer = (int)g.IdStudentNavigation.IdCareer,
                    })
                    .ToListAsync();

                return Ok(estudiantesUsuariosInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            try
            {
                var estudianteUsuario = await _context.Students
                    .Include(g => g.IdStudentNavigation)
                    .Where(g => g.IdStudent == id)
                    .Select(g => new StudentUsuario
                    {
                        IdStudent = g.IdStudent,
                        FirstName = g.FirstName,
                        LastName = g.LastName,
                        Score = g.Score,
                        Email = g.IdStudentNavigation.Email,
                        Password = g.IdStudentNavigation.Password,
                        Rol = g.IdStudentNavigation.Rol,
                        IdRank = g.IdRank,
                        IdAcademicUnity = g.IdStudentNavigation.IdAcademicUnity,
                        IdCareer = (int)g.IdStudentNavigation.IdCareer,
                    })
                    .FirstOrDefaultAsync();

                if (estudianteUsuario == null)
                {
                    return NotFound();
                }

                return Ok(estudianteUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // PUT: api/Students/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudentAndUser(int id, [FromBody] StudentUsuario updatedStudentUser)
        {
            try
            {
                var estudiante = await _context.Students.FirstOrDefaultAsync(g => g.IdStudent == id);

                if (estudiante == null)
                {
                    return NotFound();
                }

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                {
                    return NotFound();
                }

                estudiante.FirstName = updatedStudentUser.FirstName;
                estudiante.LastName = updatedStudentUser.LastName;
                estudiante.Score = updatedStudentUser.Score;
                usuario.Email = updatedStudentUser.Email;
                /*Password ?*/
                usuario.Rol = updatedStudentUser.Rol;
                usuario.IdAcademicUnity = updatedStudentUser.IdAcademicUnity;
                usuario.IdCareer = updatedStudentUser.IdCareer;
                usuario.IdAcademicUnity = updatedStudentUser.IdAcademicUnity;
                usuario.IdCareer = updatedStudentUser.IdCareer;

                await _context.SaveChangesAsync();

                return Ok("Estudiante actualizado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // POST: api/Students
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostStudent([FromBody] StudentUsuario gup)
        {
            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == gup.Email);
            if (existingUser != null)
            {
                return BadRequest("El correo electrónico ya está registrado.");
            }

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(gup.Password);

                    var user = new Usuario
                    {
                        Email = gup.Email,
                        Rol = gup.Rol,
                        Password = hashedPassword,
                        IdAcademicUnity = gup.IdAcademicUnity,
                        IdCareer = gup.IdCareer
                    };

                    var student = new Student
                    {
                        FirstName = gup.FirstName,
                        LastName = gup.LastName,
                        Score = gup.Score,
                        IdRank = gup.IdRank,
                        IdStudentNavigation = user
                    };

                    _context.Usuarios.Add(user);
                    _context.Students.Add(student);

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return Ok("Estudiante y usuario creados con éxito.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(g => g.IdStudent == id);
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (student == null || usuario == null)
                {
                    return NotFound();
                }

                // Elimina el gestor de la base de datos
                _context.Students.Remove(student);
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }
    }
}
