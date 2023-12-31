﻿using System;
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
    public class GestorsController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public GestorsController(BdgamificacionContext context)
        {
            _context = context;
        }

        // GET: api/Gestors
        [HttpGet]
        public async Task<IActionResult> GetAllGestorsAndUsers()
        {
            try
            {
                var gestoresUsuariosInfo = await _context.Gestors
                    .Include(g => g.IdGestorNavigation)
                    .Select(g => new GestorUser
                    {
                        IdGestor = g.IdGestor,
                        Email = g.IdGestorNavigation.Email,
                        Password = g.IdGestorNavigation.Password,
                        Rol = g.IdGestorNavigation.Rol,
                        FirstName = g.FirstName,
                        LastName = g.LastName,
                        IdAcademicUnity = g.IdGestorNavigation.IdAcademicUnity,
                        IdCareer = g.IdGestorNavigation.IdCareer ?? 0,

                    })
                    .ToListAsync();

                return Ok(gestoresUsuariosInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }
        // GET: api/Gestors/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGestorUserById(int id)
        {
            try
            {
                var gestorUsuario = await _context.Gestors
                    .Include(g => g.IdGestorNavigation)
                    .Where(g => g.IdGestor == id)
                    .Select(g => new GestorUser
                    {
                        IdGestor = g.IdGestor,
                        Email = g.IdGestorNavigation.Email,
                        Rol = g.IdGestorNavigation.Rol,
                        Password = g.IdGestorNavigation.Password,
                        FirstName = g.FirstName,
                        LastName = g.LastName,
                        IdAcademicUnity = g.IdGestorNavigation.IdAcademicUnity,
                        IdCareer = (int)g.IdGestorNavigation.IdCareer,

                    })
                    .FirstOrDefaultAsync();

                if (gestorUsuario == null)
                {
                    return NotFound();
                }

                return Ok(gestorUsuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // PUT: api/Gestors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGestorAndUser(int id, [FromBody] GestorUser updatedGestorUser)
        {
            //var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == updatedGestorUser.Email);
            //if (existingUser != null)
            //{
            //    return BadRequest("El correo electrónico ya está registrado.");
            //}
            try
            {
                var gestor = await _context.Gestors.FirstOrDefaultAsync(g => g.IdGestor == id);

                if (gestor == null)
                {
                    return NotFound(); // Devolver una respuesta HTTP 404 Not Found si no se encuentra el gestor
                }

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (usuario == null)
                {
                    return NotFound(); // Devolver una respuesta HTTP 404 Not Found si no se encuentra el usuario
                }

                // Actualiza los datos del gestor y usuario con los nuevos datos
                gestor.FirstName = updatedGestorUser.FirstName;
                gestor.LastName = updatedGestorUser.LastName;
                usuario.Email = updatedGestorUser.Email;
                usuario.Rol = updatedGestorUser.Rol;
                usuario.Password = updatedGestorUser.Password;
                usuario.IdAcademicUnity = updatedGestorUser.IdAcademicUnity;
                usuario.IdCareer = updatedGestorUser.IdCareer;

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok("Gestor y usuario actualizados con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }

        // POST: api/Gestors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> CreateGestorAndUser([FromBody] GestorUser gup)
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

                    var gestor = new Gestor
                    {
                        FirstName = gup.FirstName,
                        LastName = gup.LastName,
                        IdGestorNavigation = user
                    };


                    _context.Usuarios.Add(user);
                    _context.Gestors.Add(gestor);

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return Ok("Gestor y usuario creados con éxito.");
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }


        // DELETE: api/Gestors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGestor(int id)
        {
            try
            {
                var gestor = await _context.Gestors.FirstOrDefaultAsync(g => g.IdGestor == id);
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (gestor == null || usuario == null)
                {
                    return NotFound();
                }

                // Marca el gestor y el usuario como inactivos en lugar de eliminarlos
                gestor.IsActive = false;
                usuario.IsActive = false;

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return Ok("Gestor y usuario marcados como inactivos.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor: " + ex.Message);
            }
        }


        private bool GestorExists(int id)
        {
            return (_context.Gestors?.Any(e => e.IdGestor == id)).GetValueOrDefault();
        }
    }
}
