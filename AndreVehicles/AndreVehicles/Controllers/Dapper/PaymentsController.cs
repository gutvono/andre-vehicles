using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;

        PaymentsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("payment/dapper")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayment()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var payments = await connection.QueryAsync<Payment>(QueryFile.Query.Payment.GET);
                return Ok(payments);
            }
        }

        [HttpGet("payment/dapper/{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var payment = await connection.QueryFirstOrDefaultAsync<Payment>(QueryFile.Query.Payment.GETBYID, new { Id = id });

                if (payment == null)
                {
                    return NotFound();
                }

                return Ok(payment);
            }
        }

        [HttpPut("payment/dapper/{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id.Equals(payment.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Payment.UPDATE, payment);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("payment/dapper/")]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(QueryFile.Query.Payment.INSERT, payment);

                payment.Id = id;

                return CreatedAtAction("GetPayment", new { Id = payment.Id }, payment);
            }
        }

        [HttpDelete("payment/dapper/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(QueryFile.Query.Payment.DELETE, new { Id = id });

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> PaymentExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.ExecuteScalarAsync<bool>(QueryFile.Query.Payment.EXISTS, new { Id = id });
            }
        }
    }
}
