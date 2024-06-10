using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
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

        [HttpGet("sale/dapper")]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSale()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sales = await connection.QueryAsync<Sale>(QueryFile.Query.Sale.GET);
                return Ok(sales);
            }
        }

        [HttpGet("sale/dapper/{id}")]
        public async Task<ActionResult<Sale>> GetSale(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sale = await connection.QueryFirstOrDefaultAsync<Sale>(QueryFile.Query.Sale.GETBYID, new { Id = id });

                if (sale == null)
                {
                    return NotFound();
                }

                return Ok(sale);
            }
        }

        [HttpPut("sale/dapper/{id}")]
        public async Task<IActionResult> PutSale(int id, Sale sale)
        {
            if (id.Equals(sale.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Sale.UPDATE, sale);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("sale/dapper/")]
        public async Task<ActionResult<Sale>> PostSale(Sale sale)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(QueryFile.Query.Sale.INSERT, sale);

                sale.Id = id;

                return CreatedAtAction("GetSale", new { Id = sale.Id }, sale);
            }
        }

        [HttpDelete("sale/dapper/{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Sale.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Sale.EXISTS, new { Id = id });
            }
        }
    }
}
