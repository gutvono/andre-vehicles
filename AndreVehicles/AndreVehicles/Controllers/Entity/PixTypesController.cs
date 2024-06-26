﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AndreVehicles.Data;
using Model;

namespace AndreVehicles.Controllers.Entity
{
    [Route("api/[controller]")]
    [ApiController]
    public class PixTypesController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public PixTypesController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("entity/")]
        public async Task<ActionResult<IEnumerable<PixType>>> GetPixType()
        {
            return await _context.PixType.ToListAsync();
        }

        [HttpGet("entity/{id}")]
        public async Task<ActionResult<PixType>> GetPixType(int id)
        {
            var pixType = await _context.PixType.FindAsync(id);

            if (pixType == null)
            {
                return NotFound();
            }

            return pixType;
        }

        [HttpPut("entity/{id}")]
        public async Task<IActionResult> PutPixType(int id, PixType pixType)
        {
            if (id != pixType.Id)
            {
                return BadRequest();
            }

            _context.Entry(pixType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PixTypeExists(id))
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

        [HttpPost("entity/")]
        public async Task<ActionResult<PixType>> PostPixType(PixType pixType)
        {
            _context.PixType.Add(pixType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPixType", new { id = pixType.Id }, pixType);
        }

        [HttpDelete("entity/{id}")]
        public async Task<IActionResult> DeletePixType(int id)
        {
            var pixType = await _context.PixType.FindAsync(id);
            if (pixType == null)
            {
                return NotFound();
            }

            _context.PixType.Remove(pixType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PixTypeExists(int id)
        {
            return _context.PixType.Any(e => e.Id == id);
        }
    }
}
