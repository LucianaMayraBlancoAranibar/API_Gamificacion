using Gamificacion_API.Data;
using Gamificacion_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gamificacion_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentUsuarioController : ControllerBase
    {
        private readonly BdgamificacionContext _context;
        public StudentUsuarioController(BdgamificacionContext context)
        {
            _context = context;
        }
        // GET: api/<StudentUsuarioController>
        [HttpGet]
        public async Task<IActionResult> GetAllStudentUser()
        {
            try
            {
                var studentUsuariosInfo = await _context.Students
                    .Include(g => g.IdStudentNavigation)
                    .Select(g => new StudentUsuario
                    {
                        IdStudent = g.IdStudent,
                        Score = g.Score,
                        IdRank = g.IdRank,
                        FirstName = g.FirstName,
                        LastName = g.LastName,
                        Email = g.IdStudentNavigation.Email,
                        Rol = g.IdStudentNavigation.Rol,
                        IdCareer = (int)g.IdStudentNavigation.IdCareer,
                        Password = g.IdStudentNavigation.Password,
                    })
                    .ToListAsync();

                return Ok(studentUsuariosInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // GET api/<StudentUsuarioController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentUserById(int id)
        {
            try
            {
                var studentUsuario = await _context.Students
                    .Include(g => g.IdStudentNavigation)
                    .Where(g => g.IdStudent == id)
                    .Select(g => new StudentUsuario
                    {
                        IdStudent = g.IdStudent,
                        Score = g.Score,
                        IdRank = g.IdRank,
                        FirstName = g.FirstName,
                        LastName = g.LastName,
                        Email = g.IdStudentNavigation.Email,
                        Rol = g.IdStudentNavigation.Rol,
                        IdCareer = (int)g.IdStudentNavigation.IdCareer,
                        Password = g.IdStudentNavigation.Password,
                    })
                    .FirstOrDefaultAsync();

                if (studentUsuario == null)
                {
                    return NotFound();
                }

                return Ok(studentUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // POST api/<StudentUsuarioController>
        [HttpPost]
        public async Task<IActionResult> CreateStudentUser([FromBody] StudentUsuario postStudentUser)
        {
            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == postStudentUser.Email);
            if (existingUser != null)
            {
                return BadRequest("El correo electrónico ya está registrado.");
            }

            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var user = new Usuario
                    {
                        Email = postStudentUser.Email,
                        Rol = postStudentUser.Rol,
                        Password = postStudentUser.Password,
                        IdAcademicUnity = postStudentUser.IdAcademicUnity,
                        IdCareer = postStudentUser.IdCareer
                    };

                    var student = new Student
                    {
                        FirstName = postStudentUser.FirstName,
                        LastName = postStudentUser.LastName,
                        IdStudentNavigation = user,
                        IdRank = postStudentUser.IdRank,
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

        // PUT api/<StudentUsuarioController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudentUser(int id, [FromBody] StudentUsuario updateStudentUsuario)
        {
            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(g => g.IdStudent == id);

                if (student == null)
                {
                    return NotFound(); 
                }

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                {
                    return NotFound(); 
                }

         
                student.FirstName = updateStudentUsuario.FirstName;
                student.LastName = updateStudentUsuario.LastName;
                usuario.Email = updateStudentUsuario.Email;
                usuario.Rol = updateStudentUsuario.Rol;
                usuario.Password = updateStudentUsuario.Password;
                usuario.IdAcademicUnity = updateStudentUsuario.IdAcademicUnity;
                usuario.IdCareer = updateStudentUsuario.IdCareer;

                await _context.SaveChangesAsync();

                return Ok("Gestor y usuario actualizados con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // DELETE api/<StudentUsuarioController>/5
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
        private bool StudentExists(int id)
        {
            return (_context.Students?.Any(e => e.IdStudent == id)).GetValueOrDefault();
        }
    }

}
