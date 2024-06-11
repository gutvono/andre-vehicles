using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        SalesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSale()
        {
            var sales = new List<Sale>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Sale.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        sales.Add(new Sale
                        {
                            Id = reader.GetInt32(0),
                            Car = new Car { Plate = reader.GetString(1) },
                            SaleDate = reader.GetDateTime(2),
                            SaleValue = reader.GetDecimal(3),
                            Customer = new Customer { Document = reader.GetString(4) },
                            Employee = new Employee { Document = reader.GetString(5) },
                            Payment = new Payment { Id = reader.GetInt32(6) }
                        });
                    }
                }
            }
            return Ok(sales);
        }

        [HttpGet("adonet/{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            Sale sale = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Sale.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        sale = new Sale
                        {
                            Id = reader.GetInt32(0),
                            Car = new Car { Plate = reader.GetString(1) },
                            SaleDate = reader.GetDateTime(2),
                            SaleValue = reader.GetDecimal(3),
                            Customer = new Customer { Document = reader.GetString(4) },
                            Employee = new Employee { Document = reader.GetString(5) },
                            Payment = new Payment { Id = reader.GetInt32(6) }
                        };
                    }
                }
            }

            if (sale == null)
            {
                return NotFound();
            }

            return Ok(sale);
        }

        [HttpPut("adonet/{id}")]
        public async Task<IActionResult> PutSale(int id, Sale sale)
        {
            if (id != sale.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Sale.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", sale.Id);
                cmd.Parameters.AddWithValue("@Car", sale.Car.Plate);
                cmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                cmd.Parameters.AddWithValue("@SaleValue", sale.SaleValue);
                cmd.Parameters.AddWithValue("@Customer", sale.Customer.Document);
                cmd.Parameters.AddWithValue("@Employee", sale.Employee.Document);
                cmd.Parameters.AddWithValue("@Payment", sale.Payment.Id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Sale.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Car", sale.Car.Plate);
                cmd.Parameters.AddWithValue("@SaleDate", sale.SaleDate);
                cmd.Parameters.AddWithValue("@SaleValue", sale.SaleValue);
                cmd.Parameters.AddWithValue("@Customer", sale.Customer.Document);
                cmd.Parameters.AddWithValue("@Employee", sale.Employee.Document);
                cmd.Parameters.AddWithValue("@Payment", sale.Payment.Id);
                var id = (int)await cmd.ExecuteScalarAsync();

                sale.Id = id;
                return CreatedAtAction("PostSale", new { id = sale.Id }, sale);
            }
        }

        [HttpDelete("adonet/{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Sale.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> SaleExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Sale.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
