using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.AdoNet
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config QueryFile;
        TicketsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Query.json"))
            {
                string json = reader.ReadToEnd();
                QueryFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = QueryFile.ConnectionString;
            }
        }

        [HttpGet("adonet")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicket()
        {
            var tickets = new List<Ticket>();
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Ticket.GET, connection);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tickets.Add(new Ticket
                        {
                            Id = reader.GetInt32(0),
                            Number = reader.GetInt32(1),
                            ExpirationDate = reader.GetDateTime(2)
                        });
                    }
                }
            }
            return Ok(tickets);
        }

        [HttpGet("adonet/{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            Ticket ticket = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Ticket.GETBYID, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        ticket = new Ticket
                        {
                            Id = reader.GetInt32(0),
                            Number = reader.GetInt32(1),
                            ExpirationDate = reader.GetDateTime(2)
                        };
                    }
                }
            }

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        [HttpPut("adonet/{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Ticket.UPDATE, connection);
                cmd.Parameters.AddWithValue("@Id", ticket.Id);
                cmd.Parameters.AddWithValue("@Number", ticket.Number);
                cmd.Parameters.AddWithValue("@ExpirationDate", ticket.ExpirationDate);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("adonet")]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Ticket.GET + "SELECT CAST(SCOPE_IDENTITY() AS INT)", connection);
                cmd.Parameters.AddWithValue("@Number", ticket.Number);
                cmd.Parameters.AddWithValue("@ExpirationDate", ticket.ExpirationDate);
                var id = (int)await cmd.ExecuteScalarAsync();

                ticket.Id = id;
                return CreatedAtAction("PostTicket", new { id = ticket.Id }, ticket);
            }
        }

        [HttpDelete("adonet/{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Ticket.DELETE, connection);
                cmd.Parameters.AddWithValue("@Id", id);

                var affectedRows = await cmd.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        private async Task<bool> TicketExists(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var cmd = new SqlCommand(QueryFile.Query.Ticket.EXISTS, connection);
                cmd.Parameters.AddWithValue("@Id", id);
                return (bool)await cmd.ExecuteScalarAsync();
            }
        }
    }
}
