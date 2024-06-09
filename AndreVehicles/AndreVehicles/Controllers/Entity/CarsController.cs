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
    public class CarsController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public CarsController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("car/entity/")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCar()
        {
            return await _context.Car.ToListAsync();
        }

        [HttpGet("car/entity/{id}")]
        public async Task<ActionResult<Car>> GetCar(string id)
        {
            var car = await _context.Car.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        [HttpPut("car/entity/{id}")]
        public async Task<IActionResult> PutCar(string id, Car car)
        {
            if (id != car.Plate)
            {
                return BadRequest();
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
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

        [HttpPost("car/entity/")]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            _context.Car.Add(car);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CarExists(car.Plate))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCar", new { id = car.Plate }, car);
        }

        [HttpDelete("car/entity/{id}")]
        public async Task<IActionResult> DeleteCar(string id)
        {
            var car = await _context.Car.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            _context.Car.Remove(car);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(string id)
        {
            return _context.Car.Any(e => e.Plate == id);
        }
    }
}
