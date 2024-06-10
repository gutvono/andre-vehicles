using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
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

        [HttpGet("payment/adonet")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayment()
        {
            var payments = new List<Payment>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Payment.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        payments.Add(new Payment
                        {
                            Id = reader.GetInt32(0),
                            Card = new Card { CardNumber = reader.GetString(1) },
                            Ticket = new Ticket { Id = reader.GetInt32(2) },
                            Pix = new Pix { Id = reader.GetInt32(3) },
                            PaymentDate = reader.GetDateTime(4)
                        });
                    }
                }
            }
            return Ok(payments);
        }

        [HttpGet("payment/adonet/{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            Payment payment = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Payment.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        payment = new Payment
                        {
                            Id = reader.GetInt32(0),
                            Card = new Card { CardNumber = reader.GetString(1) },
                            Ticket = new Ticket { Id = reader.GetInt32(2) },
                            Pix = new Pix { Id = reader.GetInt32(3) },
                            PaymentDate = reader.GetDateTime(4)
                        };
                    }
                }
            }

            if (payment == null)
            {
                return NotFound();
            }

            return Ok(payment);
        }

        [HttpPut("payment/adonet/{id}")]
        public async Task<IActionResult> PutPayment(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Payment.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", payment.Id);
                cmd.Parameters.AddWithValue("@CardNumber", payment.Card);
                cmd.Parameters.AddWithValue("@TicketId", payment.Ticket);
                cmd.Parameters.AddWithValue("@PixId", payment.Pix);
                cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("payment/adonet")]
        public async Task<ActionResult<Payment>> PostPayment(Payment payment)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Payment.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@CardNumber", payment.Card);
                cmd.Parameters.AddWithValue("@TicketId", payment.Ticket);
                cmd.Parameters.AddWithValue("@PixId", payment.Pix);
                cmd.Parameters.AddWithValue("@PaymentDate", payment.PaymentDate);
                var id = (int)await cmd.ExecuteScalarAsync();

                payment.Id = id;
                return CreatedAtAction("PostPayment", new { id = payment.Id }, payment);
            }
        }

        [HttpDelete("payment/adonet/{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Payment.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

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
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Payment.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
