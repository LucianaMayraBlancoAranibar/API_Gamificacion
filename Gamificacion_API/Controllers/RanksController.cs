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
    public class RanksController : ControllerBase
    {
        private readonly BdgamificacionContext _context;

        public RanksController(BdgamificacionContext context)
        {
            _context = context;
        }

        // GET: api/Ranks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rank>>> GetRanks()
        {
          if (_context.Ranks == null)
          {
              return NotFound();
          }
            return await _context.Ranks.ToListAsync();
        }

        // GET: api/Ranks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rank>> GetRank(int id)
        {
          if (_context.Ranks == null)
          {
              return NotFound();
          }
            var rank = await _context.Ranks.FindAsync(id);

            if (rank == null)
            {
                return NotFound();
            }

            return rank;
        }

        // PUT: api/Ranks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRank(int id, Rank rank)
        {
            if (id != rank.IdRank)
            {
                return BadRequest();
            }

            _context.Entry(rank).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RankExists(id))
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

        // POST: api/Ranks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rank>> PostRank(Rank rank)
        {
          if (_context.Ranks == null)
          {
              return Problem("Entity set 'BdgamificacionContext.Ranks'  is null.");
          }
            _context.Ranks.Add(rank);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRank", new { id = rank.IdRank }, rank);
        }

        // DELETE: api/Ranks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRank(int id)
        {
            if (_context.Ranks == null)
            {
                return NotFound();
            }
            var rank = await _context.Ranks.FindAsync(id);
            if (rank == null)
            {
                return NotFound();
            }

            _context.Ranks.Remove(rank);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RankExists(int id)
        {
            return (_context.Ranks?.Any(e => e.IdRank == id)).GetValueOrDefault();
        }
    }
}
