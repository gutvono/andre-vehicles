using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Model;
using System.Net;
using Model.DTO;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarJobsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        CarJobsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("carJob/adonet")]
        public async Task<ActionResult<IEnumerable<CarJobDTO>>> GetCarJob()
        {
            var carJobs = new List<CarJobDTO>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.CarJob.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        carJobs.Add(new CarJobDTO
                        {
                            CarPlate = reader.GetString(reader.GetOrdinal("CarPlate")),
                            JobId = reader.GetInt32(reader.GetOrdinal("JobId")),
                            Status = reader.GetBoolean(reader.GetOrdinal("Status"))
                        });
                    }
                }
            }
            return Ok(carJobs);
        }

        [HttpGet("carJob/adonet/{id}")]
        public async Task<ActionResult<CarJobDTO>> GetCarJob(int id)
        {
            CarJobDTO carJob = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.CarJob.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        carJob = new CarJobDTO
                        {
                            CarPlate = reader.GetString(reader.GetOrdinal("CarPlate")),
                            JobId = reader.GetInt32(reader.GetOrdinal("JobId")),
                            Status = reader.GetBoolean(reader.GetOrdinal("Status"))
                        };
                    }
                }
            }

            if (carJob == null)
            {
                return NotFound();
            }

            return Ok(carJob);
        }

        [HttpPut("carJob/adonet/{id}")]
        public async Task<IActionResult> PutCarJob(int id, CarJobDTO carJobDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.CarJob.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@CarPlate", carJobDTO.CarPlate);
                cmd.Parameters.AddWithValue("@JobId", carJobDTO.JobId);
                cmd.Parameters.AddWithValue("@Status", carJobDTO.Status);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }


        [HttpPost("carJob/adonet")]
        public async Task<ActionResult<CarJob>> PostCarJob(CarJobDTO carJobDTO)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var carCmd = new SqlCommand(QueryFile.Query.Car.GETBYID, connection);
                carCmd.Parameters.AddWithValue("@Plate", carJobDTO.CarPlate);
                Car car = null;
                using (var carReader = await carCmd.ExecuteReaderAsync())
                {
                    if (await carReader.ReadAsync())
                    {
                        car = new Car
                        {
                            Plate = carReader.GetString(0),
                            Name = carReader.GetString(1),
                            ModelYear = carReader.GetInt32(2),
                            FabricationYear = carReader.GetInt32(3),
                            Color = carReader.GetString(4),
                            Sold = carReader.GetBoolean(5)
                        };
                    }
                }

                var jobCmd = new SqlCommand(QueryFile.Query.Job.GETBYID, connection);
                jobCmd.Parameters.AddWithValue("@Id", carJobDTO.JobId);
                Job job = null;
                using (var jobReader = await jobCmd.ExecuteReaderAsync())
                {
                    if (await jobReader.ReadAsync())
                    {
                        job = new Job
                        {
                            Id = jobReader.GetInt32(0),
                            Description = jobReader.GetString(1)
                        };
                    }
                }

                if (car == null || job == null)
                {
                    return BadRequest("Placa do carro ou ID de serviço inválidos.");
                }

                var carJobCmd = new SqlCommand(QueryFile.Query.CarJob.INSERT, connection);
                carJobCmd.Parameters.AddWithValue("@CarPlate", carJobDTO.CarPlate);
                carJobCmd.Parameters.AddWithValue("@JobId", carJobDTO.JobId);
                carJobCmd.Parameters.AddWithValue("@Status", carJobDTO.Status);

                var id = (int)await carJobCmd.ExecuteScalarAsync();

                var carJob = new CarJob
                {
                    Id = id,
                    Car = car,
                    Job = job,
                    Status = carJobDTO.Status
                };

                return CreatedAtAction("GetCarJob", new { id = carJob.Id }, carJob);
            }
        }

        [HttpDelete("carJob/adonet/{id}")]
        public async Task<IActionResult> DeleteCarJob(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.CarJob.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> CarJobExists(string carJobNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.CarJob.EXISTS, connection);
                cmd.Parameters.AddWithValue("@CarJobNumber", carJobNumber);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}