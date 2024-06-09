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
    public class PaymentsController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public PaymentsController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("payment/entity/")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayment()
        {
            return await _context.Payment.ToListAsync();
        }

        [HttpGet("payment/entity/{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payment.FindAsync(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        [HttpPut("payment/entity/{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest();
            }

            _context.Entry(payment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentExists(id))
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

        [HttpPost("payment/entity/")]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            _context.Payment.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayment", new { id = payment.Id }, payment);
        }

        [HttpDelete("payment/entity/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.Id == id);
        }
    }
}
