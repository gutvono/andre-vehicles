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
    public class CarJobsController : ControllerBase
    {
        private readonly AndreVehiclesContext _context;

        public CarJobsController(AndreVehiclesContext context)
        {
            _context = context;
        }

        [HttpGet("entity/")]
        public async Task<ActionResult<IEnumerable<CarJob>>> GetCarJob()
        {
            return await _context.CarJob.Include(cj => cj.Car).Include(cj => cj.Job).ToListAsync();
        }

        [HttpGet("entity/{id}")]
        public async Task<ActionResult<CarJob>> GetCarJob(int? id)
        {
            var carJob = await _context.CarJob.Include(cj => cj.Car).Include(cj => cj.Job).FirstOrDefaultAsync(cj => cj.Id == id);

            if (carJob == null)
            {
                return NotFound();
            }

            return carJob;
        }

        [HttpPut("entity/{id}")]
        public async Task<IActionResult> PutCarJob(int? id, CarJob carJob)
        {
            if (id != carJob.Id)
            {
                return BadRequest();
            }

            _context.Entry(carJob).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarJobExists(id))
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
        public async Task<ActionResult<CarJob>> PostCarJob(CarJobDTO carJobDTO)
        {
            CarJob carJob = new CarJob(carJobDTO);
            carJob.Car = await _context.Car.FindAsync(carJob.Car.Plate);
            carJob.Job = await _context.Job.FindAsync(carJob.Job.Id);

            if (carJob.Car == null || carJob.Job == null) BadRequest("Placa do carro ou ID do serviço inválidos.");

            _context.CarJob.Add(carJob);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCarJob", new { id = carJob.Id }, carJob);
        }

        [HttpDelete("entity/{id}")]
        public async Task<IActionResult> DeleteCarJob(int? id)
        {
            var carJob = await _context.CarJob.FindAsync(id);
            if (carJob == null)
            {
                return NotFound();
            }

            _context.CarJob.Remove(carJob);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarJobExists(int? id)
        {
            return _context.CarJob.Any(e => e.Id == id);
        }
    }
}
