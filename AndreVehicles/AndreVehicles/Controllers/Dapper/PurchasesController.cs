using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
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

        [HttpGet("dapper")]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var purchases = await connection.QueryAsync<Purchase>(QueryFile.Query.Purchase.GET);
                return Ok(purchases);
            }
        }

        [HttpGet("dapper/{id}")]
        public async Task<ActionResult<Purchase>> GetPurchase(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var purchase = await connection.QueryFirstOrDefaultAsync<Purchase>(QueryFile.Query.Purchase.GETBYID, new { Id = id });

                if (purchase == null)
                {
                    return NotFound();
                }

                return Ok(purchase);
            }
        }

        [HttpPut("dapper/{id}")]
        public async Task<IActionResult> PutPurchase(int id, Purchase purchase)
        {
            if (id.Equals(purchase.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Purchase.UPDATE, purchase);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("dapper/")]
        public async Task<ActionResult<Purchase>> PostPurchase(Purchase purchase)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(QueryFile.Query.Purchase.INSERT, purchase);

                purchase.Id = id;

                return CreatedAtAction("GetPurchase", new { Id = purchase.Id }, purchase);
            }
        }

        [HttpDelete("dapper/{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Purchase.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Purchase.EXISTS, new { Id = id });
            }
        }
    }
}
