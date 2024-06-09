using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Model;
using Dapper;
using Newtonsoft.Json;

namespace AndreVehicles.Controllers.Dapper
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly Config DapperFile;

        TicketsController()
        {
            using (var reader = new StreamReader(@".\Controllers\Dapper\Query.json"))
            {
                string json = reader.ReadToEnd();
                DapperFile = JsonConvert.DeserializeObject<Config>(json);
                _connectionString = DapperFile.ConnectionString;
            }
        }

        [HttpGet("ticket/dapper")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetTicket()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var tickets = await connection.QueryAsync<Ticket>(DapperFile.Query.Ticket.GET);
                return Ok(tickets);
            }
        }

        [HttpGet("ticket/dapper/{id}")]
        public async Task<ActionResult<Ticket>> GetTicket(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var ticket = await connection.QueryFirstOrDefaultAsync<Ticket>(DapperFile.Query.Ticket.GETBYID, new { Id = id });

                if (ticket == null)
                {
                    return NotFound();
                }

                return Ok(ticket);
            }
        }

        [HttpPut("ticket/dapper/{id}")]
        public async Task<IActionResult> PutTicket(int id, Ticket ticket)
        {
            if (id.Equals(ticket.Id))
            {
                return BadRequest();
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Ticket.UPDATE, ticket);

                if (affectedRows == 0)
                {
                    return NotFound();
                }

                return NoContent();
            }
        }

        [HttpPost("ticket/dapper/")]
        public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var id = await connection.QuerySingleAsync<int>(DapperFile.Query.Ticket.INSERT, ticket);

                ticket.Id = id;

                return CreatedAtAction("GetTicket", new { Id = ticket.Id }, ticket);
            }
        }

        [HttpDelete("ticket/dapper/{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var affectedRows = await connection.ExecuteAsync(DapperFile.Query.Ticket.DELETE, new { Id = id });

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
                return await connection.ExecuteScalarAsync<bool>(DapperFile.Query.Ticket.EXISTS, new { Id = id });
            }
        }
    }
}
