using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Model.DTO;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarJobsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config DapperFile;

        public CarJobsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("carjob/dapper")]
        public async Task<ActionResult<IEnumerable<CarJobDTO>>> GetCarJob()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var carJobs = await connection.QueryAsync<CarJob, Car, Job, CarJob>(
                        DapperFile.Query.CarJob.GET,
                        (carJob, car, job) =>
                        {
                            carJob.Car = car;
                            carJob.Job = job;
                            return carJob;
                        },
                        splitOn: "Plate,Id"
                    );
                return Ok(carJobs);
            }
        }

        [HttpGet("carjob/dapper/{id}")]
        public async Task<ActionResult<CarJob>> GetCarJob(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var carJob = await connection.QueryAsync<CarJob, Car, Job, CarJob>(
                    DapperFile.Query.CarJob.GETBYID,
                    (carJob, car, job) =>
                    {
                        carJob.Car = car;
                        carJob.Job = job;
                        return carJob;
                    },
                    new { Id = id },
                    splitOn: "Plate,Id"
                );

                if (carJob == null)
                {
                    return NotFound();
                }

                return Ok(carJob);
            }
        }

        [HttpPut("carjob/dapper/{id}")]
        public async Task<IActionResult> PutCarJob(int id, CarJob carJob)
        {
            if (id != carJob.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.CarJob.UPDATE, carJob);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("carjob/dapper/")]
        public async Task<ActionResult<CarJob>> PostCarJob(CarJobDTO carJobDTO)
        {
            CarJob carJob = new CarJob(carJobDTO);


            using (var connection = new SqlConnection(_connectionString))
            {
                carJob.Car = await connection.QueryFirstOrDefaultAsync<Car>(DapperFile.Query.Car.GETBYID, new { Plate = carJobDTO.CarPlate });
                carJob.Job = await connection.QueryFirstOrDefaultAsync<Job>(DapperFile.Query.Job.GETBYID, new { Id = carJobDTO.JobId });

                if (carJob.Car == null || carJob.Job == null) BadRequest("Placa do carro ou ID do serviço inválidos.");

                var id = await connection.QuerySingleAsync<int>(DapperFile.Query.CarJob.INSERT, carJob);

                carJob.Id = id;

                return CreatedAtAction("GetCarJob", new { id = carJob.Id }, carJob);
            }
        }

        [HttpDelete("carjob/dapper/{id}")]
        public async Task<IActionResult> DeleteCarJob(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.CarJob.DELETE, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> CarJobExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.CarJob.EXISTS, new { Id = id });
            }
        }
    }
}
