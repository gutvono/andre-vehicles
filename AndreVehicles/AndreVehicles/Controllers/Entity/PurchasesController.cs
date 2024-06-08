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
    public class PurchasesController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public PurchasesController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("purchases/entity/")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchase()
        {
            return await _context.Purchase.ToListAsync();
        }

        [HttpGet("purchases/entity/{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int? id)
        {
            var purchase = await _context.Purchase.FindAsync(id);

            if (purchase == null)
            {
                return NotFound();
            }

            return purchase;
        }

        [HttpPut("purchases/entity/{id}")]
        public async Task<IActionResult> PutPurchase(int? id, Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchase).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseExists(id))
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

        [HttpPost("purchases/entity/")]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            _context.Purchase.Add(purchase);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchase", new { id = purchase.Id }, purchase);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchase(int? id)
        {
            var purchase = await _context.Purchase.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            _context.Purchase.Remove(purchase);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PurchaseExists(int? id)
        {
            return _context.Purchase.Any(e => e.Id == id);
        }
    }
}
