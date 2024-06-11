using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Model;
using System.Net;
using System.Drawing;
using System.Data;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        CarsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCar()
        {
            var cars = new List<Car>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Car.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        cars.Add(new Car
                        {
                            Plate = reader.GetString(0),
                            Name = reader.GetString(1),
                            ModelYear = reader.GetInt32(2),
                            FabricationYear = reader.GetInt32(3),
                            Color = reader.GetString(4),
                            Sold = reader.GetBoolean(5)
                        });
                    }
                }
            }
            return Ok(cars);
        }

        [HttpGet("adonet/{Plate}")]
        public async Task<ActionResult<Car>> GetCar(string plate)
        {
            Car car = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Car.GETBYID, connection);
                cmd.Parameters.AddWithValue("@CarNumber", plate);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        car = new Car
                        {
                            Plate = reader.GetString(0),
                            Name = reader.GetString(1),
                            ModelYear = reader.GetInt32(2),
                            FabricationYear = reader.GetInt32(3),
                            Color = reader.GetString(4),
                            Sold = reader.GetBoolean(5)
                        };
                    }
                }
            }

            if (car == null)
            {
                return NotFound();
            }

            return Ok(car);
        }

        [HttpPut("adonet/{Plate}")]
        public async Task<IActionResult> PutCar(string plate, Car car)
        {
            if (plate != car.Plate)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Car.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Plate", car.Plate);
                cmd.Parameters.AddWithValue("@Name", car.Name);
                cmd.Parameters.AddWithValue("@ModelYear", car.ModelYear);
                cmd.Parameters.AddWithValue("@FabricationYear", car.FabricationYear);
                cmd.Parameters.AddWithValue("@Color", car.Color);
                cmd.Parameters.AddWithValue("@Sold", car.Sold);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Car.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Plate", car.Plate);
                cmd.Parameters.AddWithValue("@Name", car.Name);
                cmd.Parameters.AddWithValue("@ModelYear", car.ModelYear);
                cmd.Parameters.AddWithValue("@FabricationYear", car.FabricationYear);
                cmd.Parameters.AddWithValue("@Color", car.Color);
                cmd.Parameters.AddWithValue("@Sold", car.Sold);
                var plate = (string)await cmd.ExecuteScalarAsync();

                car.Plate = plate;
                return CreatedAtAction("PostCar", new { Plate = car.Plate }, car);
            }
        }

        [HttpDelete("adonet/{Plate}")]
        public async Task<IActionResult> DeleteCar(string plate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Car.DELETE, connection);
                cmd.Parameters.AddWithValue("@Plate", plate);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> CarExists(string plate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Car.EXISTS, connection);
                cmd.Parameters.AddWithValue("@CarNumber", plate);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}