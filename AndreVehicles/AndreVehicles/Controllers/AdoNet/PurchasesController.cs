using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        PurchasesController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("purchase/adonet")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchase()
        {
            var purchases = new List<Purchase>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Purchase.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        purchases.Add(new Purchase
                        {
                            Id = reader.GetInt32(0),
                            Car = new Car { Plate = reader.GetString(1) },
                            Price = reader.GetDecimal(2),
                            PurchaseDate = reader.GetDateTime(3)
                        });
                    }
                }
            }
            return Ok(purchases);
        }

        [HttpGet("purchase/adonet/{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            Purchase purchase = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Purchase.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        purchase = new Purchase
                        {
                            Id = reader.GetInt32(0),
                            Car = new Car { Plate = reader.GetString(1) },
                            Price = reader.GetDecimal(2),
                            PurchaseDate = reader.GetDateTime(3)
                        };
                    }
                }
            }

            if (purchase == null)
            {
                return NotFound();
            }

            return Ok(purchase);
        }

        [HttpPut("purchase/adonet/{id}")]
        public async Task<IActionResult> PutPurchase(int id, Purchase purchase)
        {
            if (id != purchase.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Purchase.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", purchase.Id);
                cmd.Parameters.AddWithValue("@Car", purchase.Car.Plate);
                cmd.Parameters.AddWithValue("@Price", purchase.Price);
                cmd.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("purchase/adonet")]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Purchase.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Car", purchase.Car.Plate);
                cmd.Parameters.AddWithValue("@Price", purchase.Price);
                cmd.Parameters.AddWithValue("@PurchaseDate", purchase.PurchaseDate);
                var id = (int)await cmd.ExecuteScalarAsync();

                purchase.Id = id;
                return CreatedAtAction("PostPurchase", new { id = purchase.Id }, purchase);
            }
        }

        [HttpDelete("purchase/adonet/{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Purchase.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> PurchaseExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Purchase.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
