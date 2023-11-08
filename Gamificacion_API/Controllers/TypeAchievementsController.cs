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
    public class TypeAchievementsController : ControllerBase
    {
        private readonly string _rutaServidor;
        private readonly BdgamificacionContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TypeAchievementsController(BdgamificacionContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;

            
            _rutaServidor = _configuration.GetSection("Configuracion:RutaServidor").Value;
        }

        // GET: api/TypeAchievements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeAchievement>>> GetTypeAchievements()
        {
          if (_context.TypeAchievements == null)
          {
              return NotFound();
          }
            return await _context.TypeAchievements.ToListAsync();
        }

        // GET: api/TypeAchievements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeAchievement>> GetTypeAchievement(int id)
        {
          if (_context.TypeAchievements == null)
          {
              return NotFound();
          }
            var typeAchievement = await _context.TypeAchievements.FindAsync(id);

            if (typeAchievement == null)
            {
                return NotFound();
            }

            return typeAchievement;
        }

        // PUT: api/TypeAchievements/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTypeAchievement(int id, TypeAchievement typeAchievement)
        {
            if (id != typeAchievement.IdTypeAchievement)
            {
                return BadRequest();
            }

            _context.Entry(typeAchievement).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeAchievementExists(id))
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


        [HttpPost]
        public async Task<ActionResult<TypeAchievement>> PostTypeAchievement([FromForm] TypeAchievementUploadModel inputModel)
        {
            try
            {
                if (inputModel == null || inputModel.Image == null || string.IsNullOrEmpty(inputModel.NameTypeAchievement) || inputModel.IdAdministrator == 0)
                {
                    return BadRequest("Solicitud inválida");
                }

        
                if (string.IsNullOrEmpty(_rutaServidor))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "La ruta del servidor no está configurada.");
                }

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(inputModel.Image.FileName);

                
                string rutaCompleta = Path.Combine(_rutaServidor, nombreArchivo);

               
                using (var fileStream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    inputModel.Image.CopyTo(fileStream);
                }

  
                string nombreTipoAchievement = inputModel.NameTypeAchievement;
                int idAdministrator = inputModel.IdAdministrator;

                var typeAchievement = new TypeAchievement
                {
                    NameTypeAchievement = nombreTipoAchievement,
                    Route = _configuration["Configuracion:BaseUrlImagenes"] + "/" + nombreArchivo,
                    IdAdministrator = idAdministrator
                };

                _context.TypeAchievements.Add(typeAchievement);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTypeAchievement", new { id = typeAchievement.IdTypeAchievement }, typeAchievement);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }


        // DELETE: api/TypeAchievements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeAchievement(int id)
        {
            if (_context.TypeAchievements == null)
            {
                return NotFound();
            }
            var typeAchievement = await _context.TypeAchievements.FindAsync(id);
            if (typeAchievement == null)
            {
                return NotFound();
            }

            _context.TypeAchievements.Remove(typeAchievement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TypeAchievementExists(int id)
        {
            return (_context.TypeAchievements?.Any(e => e.IdTypeAchievement == id)).GetValueOrDefault();
        }
    }
}
