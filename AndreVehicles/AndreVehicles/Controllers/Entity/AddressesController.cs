using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AndreVehicles.Data;
using Model;
using Model.DTO;

namespace AndreVehicles.Controllers.Entity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public AddressesController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("address/entity")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddress()
        {
            return await _context.Address.ToListAsync();
        }

        [HttpGet("address/entity/{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Address.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        [HttpPut("address/entity/{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.Id)
            {
                return BadRequest();
            }

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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

        [HttpPost("address/entity/")]
        public async Task<ActionResult<Address>> PostAddress(AddressDTO addressDto)
        {
            _context.Address.Add(new Address
            {
                Street = addressDto.Street, 
                PostalCode = addressDto.PostalCode,
                Neighborhood = addressDto.Neighborhood,
                StreetType = addressDto.StreetType,
                Number = addressDto.Number,
                Complement = addressDto.Complement,
                State = addressDto.State,
                City = addressDto.City
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddress", addressDto);
        }

        [HttpDelete("address/entity/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Address.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Address.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.Address.Any(e => e.Id == id);
        }
    }
}
