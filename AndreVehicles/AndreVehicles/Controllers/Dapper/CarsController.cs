using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
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

        [HttpGet("car/dapper")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCar()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var car = await connection.QueryAsync<Car>(QueryFile.Query.Car.GET);
                return Ok(car);
            }
        }

        [HttpGet("car/dapper/{plate}")]
        public async Task<ActionResult<Car>> GetCar(string plate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var car = await connection.QueryFirstOrDefaultAsync<Car>(QueryFile.Query.Car.GETBYID, new { Plate = plate });

                if (car == null)
                {
                    return NotFound();
                }

                return Ok(car);
            }
        }

        [HttpPut("car/dapper/{plate}")]
        public async Task<IActionResult> PutCar(string plate, Car car)
        {
            if (plate.Equals(car.Plate))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Car.UPDATE, car);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("car/dapper/")]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var plate = await connection.QuerySingleAsync<string>(QueryFile.Query.Car.INSERT, car);

                car.Plate = plate;

                return CreatedAtAction("GetCar", new { Plate = car.Plate }, car);
            }
        }

        [HttpDelete("car/dapper/{plate}")]
        public async Task<IActionResult> DeleteCar(string plate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Car.DELETE, new { Plate = plate });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Car.EXISTS, new { Plate = plate });
            }
        }
    }
}
