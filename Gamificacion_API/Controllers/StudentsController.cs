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
        //public List<RankRule> rankRules;

        public StudentsController(BdgamificacionContext context)
        {
            _context = context;
        }

        private List<RankRule> rankRules = new List<RankRule>
        {
            new RankRule { MinScore = 0, RankId = 5 },
            new RankRule { MinScore = 500, RankId = 6 },
            new RankRule { MinScore = 1000, RankId = 7 },
            new RankRule { MinScore = 1500, RankId = 8 },
            new RankRule { MinScore = 2000, RankId = 9 },
            new RankRule { MinScore = 2500, RankId = 10 },
            new RankRule { MinScore = 3000, RankId = 11 },
            new RankRule { MinScore = 3500, RankId = 12 },
            new RankRule { MinScore = 4000, RankId = 13 },
            new RankRule { MinScore = 4500, RankId = 14 },
            new RankRule { MinScore = 5000, RankId = 15 },
            new RankRule { MinScore = 5500, RankId = 16 },
            new RankRule { MinScore = 6000, RankId = 17 },
            new RankRule { MinScore = 6500, RankId = 18 },
            new RankRule { MinScore = 7000, RankId = 19 },
        };

        [HttpGet("{id}/RankS")]
        public async Task<ActionResult<StudentProfileDto>> RankS(int id)
        {
            var rank = await _context.Students
                .Where(s => s.IdStudent == id)
                .Select(s => new StudentProfileDto
                {
                    RankName = s.IdRankNavigation.NameRank ,
                    RankLevel = s.IdRankNavigation.NameSubrank, 
                    ImagePath = s.IdRankNavigation.ImagePath 
                })
                .FirstOrDefaultAsync();

            if (rank == null)
            {
                return NotFound();
            }

            return rank;
        }
 
        [HttpGet("{id}/GetStudentRank")]
        public async Task<ActionResult<StudentRankDto>> GetStudentRank(int id)
        {
            var student = await _context.Students
                .Include(s => s.IdRankNavigation)
                .Include(s => s.StudentAchievements)  
                //.Include(s => s.BadgeStudents) 
                .FirstOrDefaultAsync(s => s.IdStudent == id);

            if (student == null)
            {
                return NotFound();
            }

           
            int currentLevel = CalculateCurrentRank(student.Score); 
            int achievements = student.StudentAchievements.Count; 
            //int activityCompletions = student.BadgeStudents.Count; 

            var rankDto = new StudentRankDto
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Score = student.Score,
                RankName = student.IdRankNavigation?.NameRank,
                SubRankName = student.IdRankNavigation?.NameSubrank,
                ImagePath = student.IdRankNavigation?.ImagePath,
                AchievementsCount = student.StudentAchievements.Count,
                CurrentLevel = currentLevel,
                Achievements = achievements,
                //ActivityCompletions = activityCompletions
            };

            return Ok(rankDto);
        }

        private int CalculateCurrentRank(int score)
        {
            var currentRank = rankRules.LastOrDefault(rule => score >= rule.MinScore);
            return currentRank?.RankId ?? rankRules.First().RankId;
        }
        [HttpGet("{id}/GetStudentProfile")]
        public async Task<ActionResult<StudentProfileDto>> GetStudentProfile(int id)
        {
            var student = await _context.Students
                .Include(s => s.IdStudentNavigation)
                    .ThenInclude(u => u.IdCareerNavigation)
                        .ThenInclude(c => c.IdDepartmentNavigation)
                            .ThenInclude(d => d.IdFacultyNavigation)
                .FirstOrDefaultAsync(s => s.IdStudent == id);

            if (student == null)
            {
                return NotFound();
            }

            var userProfile = student.IdStudentNavigation;
            var career = userProfile.IdCareerNavigation;
            var department = career?.IdDepartmentNavigation;
            var faculty = department?.IdFacultyNavigation;
            var academicUnit = userProfile.IdAcademicUnityNavigation; 

            var studentProfileDto = new StudentProfileDto
            {
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = userProfile.Email,
                Career = career?.CareerName,
                Faculty = faculty?.FacultyName,
                Department = department?.DepartmentName,
                AcademicUnit = academicUnit?.AcademicUnityName 
            };

            return studentProfileDto;
        }
        [HttpGet("{id}/achievements")]
        public async Task<ActionResult<IEnumerable<AchievementUpdateDto>>> GetStudentAchievements(int id)
        {
            var studentWithAchievements = await _context.Students
         .Where(s => s.IdStudent == id)
         .SelectMany(s => s.StudentAchievements)
         .Select(sa => new AchievementUpdateDto
         {
             IdAchievement = sa.IdAchievementNavigation.IdAchievement,
             NameAchievemt = sa.IdAchievementNavigation.NameAchievemt,
             Punctuation = sa.IdAchievementNavigation.Punctuation,
             ProjectName = sa.IdAchievementNavigation.ProjectName,
             TypeName = sa.IdAchievementNavigation.IdTypeAchievementNavigation.NameTypeAchievement 
         })
         .ToListAsync();

            if (!studentWithAchievements.Any())
            {
                return NotFound();
            }

            return studentWithAchievements;
        }

        public class SanctionCreationRequest2
        {
            public int IdSanction { get; set; }
            public int IdStudent { get; set; }
            public string? StudentName { get; set; }
            public string? StudentLastName { get; set; }
            public string SanctionDescription { get; set; }
            public int SanctionValue { get; set; }
            public int ResponsibleGestorId { get; set; } // ID del gestor o administrador 
        }
        [HttpGet("{id}/sanctions")]
        public async Task<ActionResult<IEnumerable<SanctionCreationRequest2>>> GetStudentSanctions(int id)
        {
            var studentSanctions = await _context.Students
                .Where(s => s.IdStudent == id)
                .SelectMany(s => s.Sanctions)
                .Select(sanction => new SanctionCreationRequest2
                {
                    IdSanction = sanction.IdSanctions,
                    SanctionDescription = sanction.Description,
                    SanctionValue = sanction.Sanction1
                })
                .ToListAsync();

            if (!studentSanctions.Any())
            {
                return NotFound();
            }

            return studentSanctions;
        }
        public partial class BadgeStudent2
        {

            public string? BadgeName { get; set; } = null;
            public string? BadgeLevel { get; set; } = null;
            public string? ImagePath { get; set; } = null;

        }

        [HttpGet("{id}/badges")]
        public async Task<ActionResult<IEnumerable<BadgeStudent2>>> GetStudentBadges(int id)
        {
            var badges = await _context.BadgeStudents
                .Where(bs => bs.IdStudent == id)
                .Select(bs => new BadgeStudent2
                {
                    BadgeName = bs.IdBadgeNavigation.BadgeName,
                    BadgeLevel = bs.IdBadgeNavigation.BadgeLevel,
                    ImagePath = bs.IdBadgeNavigation.ImagePath 
                })
                .ToListAsync();

            if (!badges.Any())
            {
                return NotFound();
            }

            return badges;
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
        public class StudentDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public int Score { get; set; }
            public string RankName { get; set; }
            public string RankLevel { get; set; } 
                                                  
        }

        [HttpGet("GetStudentsByRank/{rankId}")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsByRank(int rankId)
        {
            
            var students = await _context.Students
                .ToListAsync();

            
            var studentsInRank = students
                .Where(s => CalculateCurrentRank(s.Score) == rankId)
                .Select(s => new StudentDto
                {
                    Id = s.IdStudent,
                    Name = s.FirstName + " " + s.LastName,
                  //  Email = s.Email, 
                    Score = s.Score,
                    RankName = s.IdRankNavigation?.NameRank, 
                    RankLevel = s.IdRankNavigation?.NameSubrank 
                                                                
                });

            if (!studentsInRank.Any())
            {
                return NotFound("No hay estudiantes en este rango.");
            }

            return Ok(studentsInRank);
        }

        private bool StudentExists(int id)
        {
            return (_context.Students?.Any(e => e.IdStudent == id)).GetValueOrDefault();
        }
    }
}
