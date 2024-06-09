using System;
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
    public class PixesController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public PixesController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("pixe/entity/")]
        public async Task<ActionResult<IEnumerable<Pix>>> GetPix()
        {
            return await _context.Pix.ToListAsync();
        }

        [HttpGet("pixe/entity/{id}")]
        public async Task<ActionResult<Pix>> GetPix(int id)
        {
            var pix = await _context.Pix.FindAsync(id);

            if (pix == null)
            {
                return NotFound();
            }

            return pix;
        }

        [HttpPut("pixe/entity/{id}")]
        public async Task<IActionResult> PutPix(int id, Pix pix)
        {
            if (id != pix.Id)
            {
                return BadRequest();
            }

            _context.Entry(pix).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PixExists(id))
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

        [HttpPost("pixe/entity/")]
        public async Task<ActionResult<Pix>> PostPix(Pix pix)
        {
            _context.Pix.Add(pix);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPix", new { id = pix.Id }, pix);
        }

        [HttpDelete("pixe/entity/{id}")]
        public async Task<IActionResult> DeletePix(int id)
        {
            var pix = await _context.Pix.FindAsync(id);
            if (pix == null)
            {
                return NotFound();
            }

            _context.Pix.Remove(pix);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PixExists(int id)
        {
            return _context.Pix.Any(e => e.Id == id);
        }
    }
}
