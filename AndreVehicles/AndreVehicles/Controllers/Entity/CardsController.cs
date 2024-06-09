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
    public class CardsController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public CardsController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("card/entity/")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCard()
        {
            return await _context.Card.ToListAsync();
        }

        [HttpGet("card/entity/{id}")]
        public async Task<ActionResult<Card>> GetCard(string id)
        {
            var card = await _context.Card.FindAsync(id);

            if (card == null)
            {
                return NotFound();
            }

            return card;
        }

        [HttpPut("card/entity/{id}")]
        public async Task<IActionResult> PutCard(string id, Card card)
        {
            if (id != card.CardNumber)
            {
                return BadRequest();
            }

            _context.Entry(card).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CardExists(id))
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

        [HttpPost("card/entity/")]
        public async Task<ActionResult<Card>> PostCard(Card card)
        {
            _context.Card.Add(card);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CardExists(card.CardNumber))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCard", new { id = card.CardNumber }, card);
        }

        [HttpDelete("card/entity/{id}")]
        public async Task<IActionResult> DeleteCard(string id)
        {
            var card = await _context.Card.FindAsync(id);
            if (card == null)
            {
                return NotFound();
            }

            _context.Card.Remove(card);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CardExists(string id)
        {
            return _context.Card.Any(e => e.CardNumber == id);
        }
    }
}
